using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Response
{
    public class ResponseGetEntireVillage : EndPointResponse
    {
        [JsonInclude]
        public required Village Village;
    }
}
