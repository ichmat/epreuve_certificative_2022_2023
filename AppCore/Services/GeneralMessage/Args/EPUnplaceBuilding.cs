using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPUnplaceBuilding : EndPointArgs
    {
        public override string Route() => APIRoute.UNPLACE_BUILDING;

        [JsonInclude]
        public int ConstructionId;

        public EPUnplaceBuilding(int constructionId)
        {
            ConstructionId = constructionId;
        }
    }
}
