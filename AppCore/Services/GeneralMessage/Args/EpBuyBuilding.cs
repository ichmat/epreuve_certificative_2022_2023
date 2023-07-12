using AppCore.Models;
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
        public int ConsInfoId;
        [JsonInclude]
        public int Type;
        [JsonInclude]
        public int Vie;
        public EpBuyBuilding(int consInfoId,int type, int vie)
        {
            ConsInfoId = consInfoId;
            Type = type;
            Vie = vie;
        }
        public override string Route() => APIRoute.CREATE_BUILDING;
    }
}
