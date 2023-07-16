using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Response
{
    public class ResponseBuyBuilding : EndPointResponse
    {
        [JsonInclude]
        public List<RessourcePossede> RessourcePossedesUpdated;

        [JsonInclude]
        public List<ObjetsPossede> ObjetsPossedesUpdated;

        public Construction ConstructionCreated { get
            {
                if(ConstructionDefCreated != null) return ConstructionDefCreated;
                if(ConstructionProdCreated != null) return ConstructionProdCreated;
                if(ConstructionAutreCreated != null) return ConstructionAutreCreated;
                throw new ArgumentNullException("Aucun bâtiment créer enregistrer");
            } 
        }

        [JsonInclude]
        public ConstructionDef? ConstructionDefCreated;

        [JsonInclude]
        public ConstructionAutre? ConstructionAutreCreated;

        [JsonInclude]
        public ConstructionProd? ConstructionProdCreated;

        public ResponseBuyBuilding(List<RessourcePossede> ressourcePossedesUpdated, List<ObjetsPossede> objetsPossedesUpdated, Construction constructionCreated)
        {
            RessourcePossedesUpdated = ressourcePossedesUpdated;
            ObjetsPossedesUpdated = objetsPossedesUpdated;
            switch (constructionCreated)
            {
                case ConstructionDef constructionDef:
                    ConstructionDefCreated = constructionDef;
                    break;
                case ConstructionAutre createdAutre:
                    ConstructionAutreCreated = createdAutre;
                    break;
                case ConstructionProd constructionProd:
                    ConstructionProdCreated = constructionProd;
                    break;
            }
        }

        [JsonConstructor]
        public ResponseBuyBuilding(List<RessourcePossede> ressourcePossedesUpdated, List<ObjetsPossede> objetsPossedesUpdated, ConstructionDef? constructionDefCreated, ConstructionAutre? constructionAutreCreated, ConstructionProd? constructionProdCreated)
        {
            RessourcePossedesUpdated = ressourcePossedesUpdated;
            ObjetsPossedesUpdated = objetsPossedesUpdated;
            ConstructionDefCreated = constructionDefCreated;
            ConstructionAutreCreated = constructionAutreCreated;
            ConstructionProdCreated = constructionProdCreated;
        }
    }
}
