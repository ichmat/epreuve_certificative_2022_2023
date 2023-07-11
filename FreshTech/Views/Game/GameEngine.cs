using AppCore.Models;
using AppCore.Property;
using AppCore.Services;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services.GeneralMessage.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Views.Game
{
    public class GameEngine
    {
        /// <summary>
        /// <b>Key</b> => La ressource <br></br>
        /// <b>Value</b> => La quantité possédé par l'utilisateur
        /// </summary>
        private Dictionary<Ressource, int> ressourceNumbers = new Dictionary<Ressource, int>();
        /// <summary>
        /// <b>Key</b> => L'objet <br></br>
        /// <b>Value</b> => La quantité possédé par l'utilisateur
        /// </summary>
        private Dictionary<Objet, int> objectsNumbers = new Dictionary<Objet, int>();
        /// <summary>
        /// <b>Key</b> => correspond à <see cref="ConstructionInfo.ConsInfoId"/> <br></br>
        /// <b>Value</b> => le schéma de construction (voir <see cref="ConstructionSchema"/>)
        /// </summary>
        private Dictionary<int, ConstructionSchema> infoId_schema = new Dictionary<int, ConstructionSchema>();
        /// <summary>
        /// Correspond à l'inventaire de bâtiment de l'utilisateur <br></br>
        /// <b>Key</b> => Le bâtiment <br></br>
        /// <b>Value</b> => Les coordonnées du bâtiment. <br></br> 
        /// </summary>
        /// <remarks>
        /// 💬<i> S'il ne possède pas de coordonnée, cela signifie que le bâtiment n'est pas sur la carte.</i>
        /// </remarks>
        private Dictionary<IConstruction, Placement?> buildings = new Dictionary<IConstruction, Placement?>();
        private Village town;

        private Attaque[] incomingAttacks;

        internal bool TownNotCreated { get; private set; } = false;

        public GameEngine()
        {
        }

        #region MODIFY_DATA

        internal async Task<bool> CreateUserVillage()
        {
            if (!TownNotCreated)
            {
                return false;
            }

            return await App.client.SendRequest(new EPCreateUserVillage());
        }

        #endregion

        #region GET_DATA

        internal IEnumerable<ConstructionSchema> GetConstructionSchemas() => infoId_schema.Values;

        internal IEnumerable<IConstruction> GetBuildingsNotInMap() => buildings.Where(x => x.Value == null).ToDictionary(x => x.Key, x => x.Value).Keys;

        internal IEnumerable<KeyValuePair<IConstruction, Placement?>> GetBuildingsInMap() => buildings.Where(x => x.Value != null);

        internal KeyValuePair<Ressource, int>[] GetRessourcesWithQuantity() => ressourceNumbers.ToArray();

        internal KeyValuePair<Objet, int>[] GetObjetsWithQuantity() => objectsNumbers.ToArray();

        internal int GetRessourceQuantity(RESSOURCE ressource) => ressourceNumbers[NecessaryData.GetRessource(ressource)];

        internal int GetObjetQuantity(OBJET objet) => objectsNumbers[NecessaryData.GetObjet(objet)];

        #endregion

        #region RELOAD

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
            Array.ForEach(NecessaryData.ressources, x => ressourceNumbers.Add(x, 0)); // mise en place des ressources
            Array.ForEach(NecessaryData.objets, x => objectsNumbers.Add(x, 0)); // mise en place des objets
            
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
            Array.ForEach(dataTown.RessourcesPossede,
                // on récupère l'instance dans NecessaryData pour éviter de le créer plusieurs fois
                x => ressourceNumbers[NecessaryData.GetRessourceById(x.RessourceId)] = x.Nombre
                );
            Array.ForEach(dataTown.ObjetsPossedes,
                // on récupère l'instance dans NecessaryData pour éviter de le créer plusieurs fois
                x => objectsNumbers[NecessaryData.GetObjetById(x.ObjetId)] = x.Nombre);

            incomingAttacks = dataTown.AttaquesActuel;
        }

        #endregion
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

    /// <summary>
    /// Schéma de construction pour un bâtiment spécifique. Possède les besoins de créations, 
    /// d'amélioration (voir <see cref="UpgradeSchema"/>) et de réparation.
    /// </summary>
    public struct ConstructionSchema
    {
        public readonly ConstructionInfo ConsInfo;
        // besoin pour effectuer la construction
        /// <summary>
        /// <b>Key</b> => l'objet nécessaire pour la création <br></br>
        /// <b>Value</b> => la quantité nécessaire pour la création
        /// </summary>
        public readonly Dictionary<Objet, int> CreationObjets;
        /// <summary>
        /// <b>Key</b> => la ressource nécessaire pour la création <br></br>
        /// <b>Value</b> => la quantité nécessaire pour la création
        /// </summary>
        public readonly Dictionary<Ressource, int> CreationRessources;
        // besoin pour effectuer une amélioration
        /// <summary>
        /// <b>Key</b> => le niveau concerné (exemple : s'il est égal à 2 cela signifie que 
        /// cela correspond au niveau 2) <br></br>
        /// <b>Value</b> => le Schéma de construction, voir <see cref="UpgradeSchema"/> pour 
        /// plus d'info
        /// </summary>
        /// <remarks>
        /// ⚠<i> Les schémas de constructions commence toujours à partir du niveau 2</i>
        /// </remarks>
        public readonly Dictionary<int, UpgradeSchema> UpgradeSchemasParNiveau;
        // besoin pour effectuer une réparation
        /// <summary>
        /// <b>Key</b> => la ressource nécessaire pour la réparation <br></br>
        /// <b>Value</b> => la quantité nécessaire pour la réparation
        /// </summary>
        public readonly Dictionary<Ressource, int> ReparationRessources;
        /// <summary>
        /// <b>Key</b> => la ressource nécessaire pour la réparation <br></br>
        /// <b>Value</b> => le facteur de multiplication à appliquer selon le niveau de construction
        /// et la quantité de ressource nécessaire
        /// </summary>
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

    /// <summary>
    /// Schéma d'amélioration pour la construction
    /// </summary>
    public struct UpgradeSchema
    {
        /// <summary>
        /// Le niveau concerné (exemple : s'il est égal à 2 cela signifie que 
        /// cela correspond au niveau 2) <br></br>
        /// </summary>
        public readonly int NiveauConcerne;
        /// <summary>
        /// <b>Key</b> => l'objet nécessaire pour l'amélioration <br></br>
        /// <b>Value</b> => la quantité nécessaire pour l'amélioration
        /// </summary>
        public readonly Dictionary<Objet, int> AmeliorationObjets;
        /// <summary>
        /// <b>Key</b> => la ressource nécessaire pour l'amélioration <br></br>
        /// <b>Value</b> => la quantité nécessaire pour l'amélioration
        /// </summary>
        public readonly Dictionary<Ressource, int> AmeliorationRessources;

        public UpgradeSchema(int niveauConcerne)
        {
            NiveauConcerne = niveauConcerne;
            AmeliorationObjets = new Dictionary<Objet, int>();
            AmeliorationRessources = new Dictionary<Ressource, int>();
        }
    }
}
