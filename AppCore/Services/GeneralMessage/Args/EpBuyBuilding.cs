using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EpBuyBuilding : EndPointArgs
    {
        [JsonInclude]
        public int consID;
        
        EpBuyBuilding(int constructionId)
        {
            consID = constructionId;
        }
        public override string Route() => APIRoute.CREATE_BUILDING;
    }
}
