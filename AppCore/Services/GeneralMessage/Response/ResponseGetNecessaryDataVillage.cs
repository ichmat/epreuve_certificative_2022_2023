using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Response
{
    public class ResponseGetNecessaryDataVillage : EndPointResponse
    {
        [JsonInclude]
        public ConstructionInfo[] ConstructionInfos;

        [JsonInclude]
        public Ressource[] Ressources;

        [JsonInclude]
        public Objet[] Objets;

        [JsonInclude]
        public CreationRessource[] CreationRessources;

        [JsonInclude]
        public AmeliorationRessource[] AmeliorationRessources;

        [JsonInclude]
        public ReparationRessource[] ReparationRessources;

        [JsonInclude]
        public CreationObjet[] CreationObjets;

        [JsonInclude]
        public AmeliorationObjet[] AmeliorationObjets;

        public ResponseGetNecessaryDataVillage(ConstructionInfo[] constructionInfos, Ressource[] ressources, Objet[] objets, CreationRessource[] creationRessources, AmeliorationRessource[] ameliorationRessources, ReparationRessource[] reparationRessources, CreationObjet[] creationObjets, AmeliorationObjet[] ameliorationObjets)
        {
            ConstructionInfos = constructionInfos;
            Ressources = ressources;
            Objets = objets;
            CreationRessources = creationRessources;
            AmeliorationRessources = ameliorationRessources;
            ReparationRessources = reparationRessources;
            CreationObjets = creationObjets;
            AmeliorationObjets = ameliorationObjets;
        }
    }
}
