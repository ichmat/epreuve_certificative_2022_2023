using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public static class IconConstruction
    {
        private readonly static ReadOnlyDictionary<int, string> consInfoId_icon;

        private const string DEFAULT_SOURCE = "question.svg";

        static IconConstruction()
        {
            IDictionary<int, string> dict = new Dictionary<int, string>() {
                { 1, "" }, // tourelle auto
                { 2, "fence.svg" }, // barrière
                { 3, "" }, // Tour de guet
                { 4, "windmillalt.svg" }, // Eolienne de fortune
                { 5, "" }, // Canon
                { 6, "eletric_central.svg" }, // Centrale électrique
                { 7, "" }, // Ferme
                { 8, "" }, // Scierie
                { 9, "" }, // Usine de traitement d'eau
                { 10, "" }, // Quartier résidentiel
                { 11, "" }, // Fonderie
                { 12, "" }, // Théâtre

            };
            consInfoId_icon = new ReadOnlyDictionary<int, string>(dict);
        }

        public static string GetIconByConsInfoId(int consInfoId)
        {
            if (!consInfoId_icon.ContainsKey(consInfoId) || string.IsNullOrWhiteSpace(consInfoId_icon[consInfoId]))
            {
                return DEFAULT_SOURCE;
            }
            return consInfoId_icon[consInfoId];
        }
    }
}
