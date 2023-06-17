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
        private List<GConstructionProd> gConstructionProds = new List<GConstructionProd>();



        internal async Task Load()
        {
            // faire en sorte de compiler les informations en plusieurs objets struct pour permettre une meilleur accessibilité et un meilleur traitement
            ResponseGetNecessaryDataVillage infoTown = await App.client.SendAndGetResponse<ResponseGetNecessaryDataVillage>(new EPGetNecessaryDataVillage());
            ResponseGetEntireVillage dataTown = await App.client.SendAndGetResponse<ResponseGetEntireVillage>(new EPGetEntireVillage());
        }
    }

    public struct GConstructionProd
    {
        public int ConsInfoId;
        public TypeConstruction Type;

        public string Nom;
        public int Vie;
        public int VieMax;
        public int Niveau;
        public int NiveauMax;

        public int Production;
        public float MultParNiveau;

        public readonly Ressource Ressource;

        public GConstructionProd(ConstructionInfo info, ConstructionProd constructionProd) 
        {
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
    }

    public struct GConstructionDef
    {
        public int ConsInfoId;
        public TypeConstruction Type;

        public string Nom;
        public int Vie;
        public int VieMax;
        public int Niveau;
        public int NiveauMax;

        public int Puissance;
        public float MultParNiveau;

        public GConstructionDef(ConstructionInfo info, ConstructionDef constructionDef)
        {
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
    }

    public struct GConstructionAutre
    {
        public int ConsInfoId;
        public TypeConstruction Type;

        public string Nom;
        public int Vie;
        public int VieMax;
        public int Niveau;
        public int NiveauMax;

        public GConstructionAutre(ConstructionInfo info, ConstructionAutre constructionAutre)
        {
            ConsInfoId = constructionAutre.ConsInfoId;
            Type = constructionAutre.Type;
            Nom = info.Nom;
            Vie = constructionAutre.Vie;
            VieMax = info.VieMax;
            Niveau = constructionAutre.Niveau;
            NiveauMax = info.NiveauMax;
        }
    }
}
