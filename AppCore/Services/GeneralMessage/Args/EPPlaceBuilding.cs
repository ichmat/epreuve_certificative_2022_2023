using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPPlaceBuilding : EndPointArgs
    {
        public override string Route() => APIRoute.PLACE_BUILDING;

        [JsonInclude]
        public int ConstructionId;

        [JsonInclude]
        public int X;

        [JsonInclude]
        public int Y;

        public EPPlaceBuilding(int constructionId, int x, int y)
        {
            ConstructionId = constructionId;
            X = x;
            Y = y;
        }
    }
}
