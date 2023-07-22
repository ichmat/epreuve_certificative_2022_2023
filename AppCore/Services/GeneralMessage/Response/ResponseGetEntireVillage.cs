using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Response
{
    public class ResponseGetEntireVillage : EndPointResponse
    {
        [JsonInclude]
        public Village Village;

        [JsonInclude]
        public ConstructionDef[] ConstructionsDef;

        [JsonInclude]
        public ConstructionProd[] ConstructionProds;

        [JsonInclude]
        public ConstructionAutre[] ConstructionAutres;

        [JsonInclude]
        public Attaque[] AttaquesActuel;

        [JsonInclude]
        public Coordonnee[] Coordonnees;

        [JsonInclude]
        public RessourcePossede[] RessourcesPossede;

        [JsonInclude]
        public ObjetsPossede[] ObjetsPossedes;

        public ResponseGetEntireVillage(Village village, ConstructionDef[] constructionsDef, ConstructionProd[] constructionProds, ConstructionAutre[] constructionAutres, Attaque[] attaquesActuel, Coordonnee[] coordonnees, RessourcePossede[] ressourcesPossede, ObjetsPossede[] objetsPossedes)
        {
            Village = village;
            ConstructionsDef = constructionsDef;
            ConstructionProds = constructionProds;
            ConstructionAutres = constructionAutres;
            AttaquesActuel = attaquesActuel;
            Coordonnees = coordonnees;
            RessourcesPossede = ressourcesPossede;
            ObjetsPossedes = objetsPossedes;
        }
    }
}
