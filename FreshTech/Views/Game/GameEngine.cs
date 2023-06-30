using AppCore.Models;
using AppCore.Property;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services.GeneralMessage.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Views.Game
{
    internal class GameEngine
    {
        private Dictionary<Ressource, int> ressourceNumbers = new Dictionary<Ressource, int>();
        private Dictionary<Objet, int> objectsNumbers = new Dictionary<Objet, int>();
        private Dictionary<int, ConstructionSchema> infoId_schema = new Dictionary<int, ConstructionSchema>();

        private Dictionary<IConstruction, Placement?> buildings = new Dictionary<IConstruction, Placement?>();
        private Village town;

        private Attaque[] incomingAttacks;

        internal bool TownNotCreated { get; private set; } = false;

        internal async Task ReloadAllData()
        {
            // faire en sorte de compiler les informations en plusieurs objets struct pour permettre une meilleur accessibilité et un meilleur traitement
            await ReloadNecessaryData();
            await ReloadUserTown();
        }

        private async Task ReloadNecessaryData()
        {
            ressourceNumbers.Clear();
            objectsNumbers.Clear();
            infoId_schema.Clear();

            ResponseGetNecessaryDataVillage infoTown = await App.client.SendAndGetResponse<ResponseGetNecessaryDataVillage>(new EPGetNecessaryDataVillage());
            Array.ForEach(infoTown.Ressources, x => ressourceNumbers.Add(x, 0)); // mise en place des ressources
            Array.ForEach(infoTown.Objets, x => objectsNumbers.Add(x, 0)); // mise en place des objets
            
            foreach(ConstructionInfo ci in infoTown.ConstructionInfos)
            {
                // mise en place des informations de constructions des bâtiments
                infoId_schema.Add(ci.ConsInfoId, new ConstructionSchema(ci,
                    infoTown.CreationObjets.Where(x => x.ConsInfoId == ci.ConsInfoId).ToArray(),
                    infoTown.CreationRessources.Where(x => x.ConsInfoId == ci.ConsInfoId).ToArray(),
                    infoTown.AmeliorationObjets.Where(x => x.ConsInfoId == ci.ConsInfoId).ToArray(),
                    infoTown.AmeliorationRessources.Where(x => x.ConsInfoId == ci.ConsInfoId).ToArray(),
                    infoTown.ReparationRessources.Where(x => x.ConsInfoId == ci.ConsInfoId).ToArray()
                    ));
            }
        }

        internal async Task ReloadUserTown()
        {
            buildings.Clear();

            ResponseGetEntireVillage dataTown = await App.client.SendAndGetResponse<ResponseGetEntireVillage>(new EPGetEntireVillage());
            if (dataTown == null)
            {
                TownNotCreated = true;
                return;
            }
            town = dataTown.Village;
            // mise en place des constructions que le joueur possède
            Array.ForEach(dataTown.ConstructionAutres, x => buildings.Add(
                new GConstructionAutre(x.ConstructionInfo, x), null));
            Array.ForEach(dataTown.ConstructionsDef, x => buildings.Add(
                new GConstructionDef(x.ConstructionInfo, x), null));
            Array.ForEach(dataTown.ConstructionProds, x => buildings.Add(
                new GConstructionProd(x.ConstructionInfo, x), null));

            // mise en place des coordonnées des constructions
            Array.ForEach(dataTown.Coordonnees, coord =>
            {
                IConstruction? construction = buildings.Keys.FirstOrDefault(x => x.GetConsId() == coord.ConstructionId);
                if (construction != null)
                {
                    buildings[construction] = new Placement(construction, coord.X, coord.Y);
                }
            });

            // comptage des ressources
            Array.ForEach(dataTown.RessourcesPossede, x => ressourceNumbers[x.Ressource] = x.Nombre);
            Array.ForEach(dataTown.ObjetsPossedes, x => objectsNumbers[x.Objet] = x.Nombre);

            incomingAttacks = dataTown.AttaquesActuel;
        }
    }

    public interface IConstruction
    {
        int GetConsId();
        int GetNivMax();
        int GetVie();
        int GetVieMax();
        TypeConstruction GetTypeConstruction();
    }

    public struct GConstructionProd : IConstruction
    {
        public int ConsId;
        public int ConsInfoId;
        public TypeConstruction Type;

        public string Nom;
        public int Vie;
        public readonly int VieMax;
        public int Niveau;
        public readonly int NiveauMax;

        public readonly int Production;
        public readonly float MultParNiveau;

        public readonly Ressource Ressource;

        public GConstructionProd(ConstructionInfo info, ConstructionProd constructionProd) 
        {
            ConsId = constructionProd.ConstructionId;
            ConsInfoId = constructionProd.ConsInfoId;
            Type = constructionProd.Type;
            Nom = info.Nom;
            Vie = constructionProd.Vie;
            VieMax = info.VieMax;
            Niveau = constructionProd.Niveau;
            NiveauMax = info.NiveauMax;
            Production = constructionProd.Production;
            MultParNiveau = constructionProd.MultParNiveau;
            // vérifier si ressource n'est pas null
            Ressource = constructionProd.Ressource;
        }

        public int GetNivMax() => NiveauMax;

        public int GetVie() => Vie;

        public int GetVieMax() => VieMax;

        public TypeConstruction GetTypeConstruction() => Type;

        public int GetConsId() => ConsId;
    }

    public struct GConstructionDef : IConstruction
    {
        public int ConsId;
        public int ConsInfoId;
        public TypeConstruction Type;

        public string Nom;
        public int Vie;
        public readonly int VieMax;
        public int Niveau;
        public readonly int NiveauMax;

        public readonly int Puissance;
        public readonly float MultParNiveau;

        public GConstructionDef(ConstructionInfo info, ConstructionDef constructionDef)
        {
            ConsId = constructionDef.ConstructionId;
            ConsInfoId = constructionDef.ConsInfoId;
            Type = constructionDef.Type;
            Nom = info.Nom;
            Vie = constructionDef.Vie;
            VieMax = info.VieMax;
            Niveau = constructionDef.Niveau;
            NiveauMax = info.NiveauMax;
            Puissance = constructionDef.Puissance;
            MultParNiveau = constructionDef.MultParNiveau;
        }

        public int GetNivMax() => NiveauMax;

        public int GetVie() => Vie;

        public int GetVieMax() => VieMax;

        public TypeConstruction GetTypeConstruction() => Type;

        public int GetConsId() => ConsId;
    }

    public struct GConstructionAutre : IConstruction
    {
        public int ConsId;
        public int ConsInfoId;
        public TypeConstruction Type;

        public string Nom;
        public int Vie;
        public readonly int VieMax;
        public int Niveau;
        public readonly int NiveauMax;

        public GConstructionAutre(ConstructionInfo info, ConstructionAutre constructionAutre)
        {
            ConsId = constructionAutre.ConstructionId;
            ConsInfoId = constructionAutre.ConsInfoId;
            Type = constructionAutre.Type;
            Nom = info.Nom;
            Vie = constructionAutre.Vie;
            VieMax = info.VieMax;
            Niveau = constructionAutre.Niveau;
            NiveauMax = info.NiveauMax;
        }

        public int GetNivMax() => NiveauMax;

        public int GetVie() => Vie;

        public int GetVieMax() => VieMax;

        public TypeConstruction GetTypeConstruction() => Type;

        public int GetConsId() => ConsId;
    }

    public struct ConstructionSchema
    {
        public readonly ConstructionInfo ConsInfo;
        // besoin pour effectuer la construction
        public readonly Dictionary<Objet, int> CreationObjets;
        public readonly Dictionary<Ressource, int> CreationRessources;
        // besoin pour effectuer une amélioration
        public readonly Dictionary<int, UpgradeSchema> UpgradeSchemasParNiveau;
        // besoin pour effectuer une réparation
        public readonly Dictionary<Ressource, int> ReparationRessources;
        public readonly Dictionary<Ressource, float> ReparationMultParNiveau;

        public ConstructionSchema(ConstructionInfo constructionInfo, IEnumerable<CreationObjet> creationObjets, IEnumerable<CreationRessource> creationRessources, 
            IEnumerable<AmeliorationObjet> ameliorationObjets, IEnumerable<AmeliorationRessource> ameliorationRessources, IEnumerable<ReparationRessource> reparationRessources)
        {
            ConsInfo = constructionInfo;
            CreationObjets = new Dictionary<Objet, int>();
            foreach (var objet in creationObjets)
            {
                CreationObjets.Add(objet.Objet, objet.Nombre);
            }
            CreationRessources = new Dictionary<Ressource, int>();
            foreach(var ressource in creationRessources)
            {
                CreationRessources.Add(ressource.Ressource, ressource.Nombre);
            }
            UpgradeSchemasParNiveau = new Dictionary<int, UpgradeSchema>();
            for (int i = 2; i <= constructionInfo.NiveauMax; i++)
            {
                UpgradeSchema schema = new UpgradeSchema(i);

                var levelUpgradeObj = ameliorationObjets.Where(x => x.NiveauConcerne == i);
                if(levelUpgradeObj.Count() > 0)
                {
                    foreach (AmeliorationObjet AO in levelUpgradeObj)
                    {
                        schema.AmeliorationObjets.Add(AO.Objet, AO.Nombre);
                    }
                }

                var levelUpgradeRes = ameliorationRessources.Where(x => x.NiveauConcerne == i);
                if(levelUpgradeRes.Count() > 0)
                {
                    foreach (AmeliorationRessource AR in levelUpgradeRes)
                    {
                        schema.AmeliorationRessources.Add(AR.Ressource, AR.Nombre);
                    }
                }
                
                UpgradeSchemasParNiveau.Add(i, schema);
            }
            ReparationRessources = new Dictionary<Ressource, int>();
            ReparationMultParNiveau = new Dictionary<Ressource, float>();
            foreach (ReparationRessource reparation in reparationRessources)
            {
                ReparationRessources.Add(reparation.Ressource, reparation.Nombre);
                ReparationMultParNiveau.Add(reparation.Ressource, reparation.MultParNiveau);
            }
        }
    }

    public struct Placement
    {
        public readonly IConstruction Construction;
        public int X;
        public int Y;

        public Placement(IConstruction construction, int x, int y)
        {
            Construction = construction;
            X = x;
            Y = y;
        }
    }

    public struct UpgradeSchema
    {
        public readonly int NiveauConcerne;
        public readonly Dictionary<Objet, int> AmeliorationObjets;
        public readonly Dictionary<Ressource, int> AmeliorationRessources;

        public UpgradeSchema(int niveauConcerne)
        {
            NiveauConcerne = niveauConcerne;
            AmeliorationObjets = new Dictionary<Objet, int>();
            AmeliorationRessources = new Dictionary<Ressource, int>();
        }
    }
}
