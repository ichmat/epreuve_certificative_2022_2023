using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Tools
{
    public static class ImgConstruction
    {
        private static readonly string[] sources =
        {
            "",
            "",
            "",
            "windmillalt.svg",
            "",
            "",
        };

        public static string GetSource(in int consInfoId)
        {
            if(consInfoId >= 0 && consInfoId < sources.Length)
            {
                return sources[consInfoId];
            }
            return string.Empty;
        }
    }
}
