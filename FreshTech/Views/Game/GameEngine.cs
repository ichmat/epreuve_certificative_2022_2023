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

        internal async Task Load()
        {
            // faire en sorte de compiler les informations en plusieurs objets struct pour permettre une meilleur accessibilité et un meilleur traitement
            ResponseGetNecessaryDataVillage infoTown = await App.client.SendAndGetResponse<ResponseGetNecessaryDataVillage>(new EPGetNecessaryDataVillage());
            ResponseGetEntireVillage dataTown = await App.client.SendAndGetResponse<ResponseGetEntireVillage>(new EPGetEntireVillage());
        }

    }

    public interface GConstruction
    {
        int GetConsId();
        int GetNivMax();
        int GetVie();
        int GetVieMax();
        TypeConstruction GetTypeConstruction();
    }

    public struct GConstructionProd : GConstruction
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

    public struct GConstructionDef : GConstruction
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

    public struct GConstructionAutre : GConstruction
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
        public readonly GConstruction Construction;
        // besoin pour effectuer la construction
        public readonly Dictionary<Objet, int> CreationObjets;
        public readonly Dictionary<Ressource, int> CreationRessources;
        // besoin pour effectuer une amélioration
        public readonly Dictionary<int, UpgradeSchema> UpgradeSchemasParNiveau;
        // besoin pour effectuer une réparation
        public readonly Dictionary<Ressource, int> ReparationRessources;
        public readonly Dictionary<Ressource, float> ReparationMultParNiveau;

        public ConstructionSchema(GConstruction construction, IEnumerable<CreationObjet> creationObjets, IEnumerable<CreationRessource> creationRessources, 
            IEnumerable<AmeliorationObjet> ameliorationObjets, IEnumerable<AmeliorationRessource> ameliorationRessources, IEnumerable<ReparationRessource> reparationRessources)
        {
            Construction = construction;
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
            for (int i = 1; i <= Construction.GetNivMax(); i++)
            {
                UpgradeSchema schema = new UpgradeSchema();
                foreach(AmeliorationObjet AO in ameliorationObjets.Where(x => x.NiveauConcerne == i))
                {
                    schema.AmeliorationObjets.Add(AO.Objet, AO.Nombre);
                }

                foreach (AmeliorationRessource AR in ameliorationRessources.Where(x => x.NiveauConcerne == i))
                {
                    schema.AmeliorationRessources.Add(AR.Ressource, AR.Nombre);
                }
                UpgradeSchemasParNiveau.Add(i, schema);
            }
            ReparationRessources = new Dictionary<Ressource, int>();
            foreach(ReparationRessource reparation in reparationRessources)
            {
                ReparationRessources.Add(reparation.Ressource, reparation.Nombre);
                ReparationMultParNiveau.Add(reparation.Ressource, reparation.MultParNiveau);
            }
        }
    }

    public struct Placement
    {
        public readonly GConstruction Construction;
        public int X;
        public int Y;
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
