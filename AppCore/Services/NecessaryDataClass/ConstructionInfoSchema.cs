using AppCore.Models;
using AppCore.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services.NecessaryDataClass
{
    internal class ConstructionInfoSchema
    {
        public int ConsInfoId;
        public string Nom;
        public int VieMax;
        public byte NiveauMax;
        public TypeConstruction Type;
        public long TempsSecConstruction;

        /// <summary>
        /// 1 => id objet <br></br>
        /// 2 => quantité 
        /// </summary>
        public Dictionary<int, int> CreationsObjet;
        /// <summary>
        /// 1 => id ressource <br></br>
        /// 2 => quantité 
        /// </summary>
        public Dictionary<int, int> CreationsRessources;
        /// <summary>
        /// 1 => niveau concerné <br></br>
        /// 2 => liste d'objets nécessaire : <br></br>
        ///     - Key : id objet <br></br>
        ///     - Value : quantité
        /// </summary>
        public Dictionary<int, List<KeyValuePair<int, int>>> AmeliorationsObjet;
        /// <summary>
        /// 1 => niveau concerné <br></br>
        /// 2 => liste de ressources nécessaire : <br></br>
        ///     - Key : id ressource <br></br>
        ///     - Value : quantité
        /// </summary>
        public Dictionary<int, List<KeyValuePair<int, int>>> AmeliorationsRessources;
        /// <summary>
        /// 1 => id ressource <br></br>
        /// 2 => Tuple avec les infos suivants : <br></br>
        ///     - int => quantité<br></br>
        ///     - float => multiplicateur par niveau<br></br>
        ///     - long => temps (en seconde) nécessaire
        /// </summary>
        public Dictionary<int, Tuple<int, float, long>> ReparationsRessources;

        public ConstructionInfoSchema(int consInfoId, string nom, int vieMax, byte niveauMax, TypeConstruction type, long tempsCons, Dictionary<int, int> creationObjet, Dictionary<int, int> creationRessources, Dictionary<int, List<KeyValuePair<int, int>>> ameliorationObjet, Dictionary<int, List<KeyValuePair<int, int>>> ameliorationRessources, Dictionary<int, Tuple<int, float, long>> reparationRessources)
        {
            ConsInfoId = consInfoId;
            Nom = nom;
            VieMax = vieMax;
            NiveauMax = niveauMax;
            Type = type;
            TempsSecConstruction = tempsCons;
            CreationsObjet = creationObjet;
            CreationsRessources = creationRessources;
            AmeliorationsObjet = ameliorationObjet;
            AmeliorationsRessources = ameliorationRessources;
            ReparationsRessources = reparationRessources;
        }

        private ConstructionInfoSchema(int consInfoId, string nom, int vieMax, TypeConstruction type, long tempsCons)
        {
            ConsInfoId = consInfoId;
            Nom = nom;
            VieMax = vieMax;
            NiveauMax = 1;
            Type = type;
            CreationsObjet = new Dictionary<int, int>();
            CreationsRessources = new Dictionary<int, int>();
            AmeliorationsObjet = new Dictionary<int, List<KeyValuePair<int, int>>>();
            AmeliorationsRessources = new Dictionary<int, List<KeyValuePair<int, int>>>();
            ReparationsRessources = new Dictionary<int, Tuple<int, float, long>>();
            TempsSecConstruction = tempsCons;
        }

        public ConstructionInfoSchema WithCreationsObjects(params Objet[] objets)
        {
            Array.ForEach(objets, (objet) => {
                if (!CreationsObjet.ContainsKey(objet.ObjetId))
                {
                    CreationsObjet.Add(objet.ObjetId, 1);
                }
                else
                {
                    CreationsObjet[objet.ObjetId]++;
                }
            });

            return this;
        }

        public ConstructionInfoSchema AddReparationRessources(Ressource ressourceNeeded, int quantity, float multParNiveau, long tempsSec)
        {
            if (!ReparationsRessources.ContainsKey(ressourceNeeded.RessourceId))
            {
                ReparationsRessources.Add(ressourceNeeded.RessourceId,
                    new Tuple<int, float, long>(quantity, multParNiveau, tempsSec));
            }
            else
            {
                throw new Exception("ressourceId already set");
            }
            return this;
        }

        public ConstructionInfoSchema AddCreationRessources(Ressource ressource, int quantity)
        {
            if (!CreationsRessources.ContainsKey(ressource.RessourceId))
            {
                CreationsRessources.Add(ressource.RessourceId, quantity);
            }
            else
            {
                CreationsRessources[quantity] += quantity;
            }

            return this;
        }

        public ConstructionInfoSchema AddLevel(Ressource[] ressourcesNeeded, int[] quantities, params Objet[] objetsNeed)
        {
            if (ressourcesNeeded.Length != quantities.Length)
                throw new ArgumentException("array len not same size", "ressourcesNeeded, quantities");

            NiveauMax++;
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < quantities.Length; ++i)
            {
                list.Add(new KeyValuePair<int, int>(ressourcesNeeded[i].RessourceId, quantities[i]));
            }
            AmeliorationsRessources.Add(NiveauMax, list);

            Dictionary<int, int> dict = new Dictionary<int, int>();

            for (int i = 0; i < objetsNeed.Length; ++i)
            {
                if (!dict.ContainsKey(objetsNeed[i].ObjetId))
                {
                    dict.Add(objetsNeed[i].ObjetId, 1);
                }
                else
                {
                    dict[objetsNeed[i].ObjetId]++;
                }
            }

            AmeliorationsObjet.Add(NiveauMax, dict.ToList());

            return this;
        }

        public static ConstructionInfoSchema Create(int consInfoId, string nom, int vieMax, TypeConstruction type, long tempsSecConstruction)
        {
            return new ConstructionInfoSchema(consInfoId, nom, vieMax, type, tempsSecConstruction);
        }
    }
}
