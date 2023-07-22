using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPBuyBuilding : EndPointArgs
    {
        [JsonInclude]
        public int ConsInfoId;
        
        public EPBuyBuilding(int consInfoId)
        {
            ConsInfoId = consInfoId;
        }
        
        public override string Route() => APIRoute.CREATE_BUILDING;
    }
}
