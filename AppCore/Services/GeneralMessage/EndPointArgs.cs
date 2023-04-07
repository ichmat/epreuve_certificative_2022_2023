using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage
{
    public abstract class EndPointArgs
    {
        [JsonInclude]
        public string Token;

        protected EndPointArgs(string token)
        {
            Token = token;
        }

        protected EndPointArgs()
        {
            Token = string.Empty;
        }

        public abstract string Route();
    }
}
