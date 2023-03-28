using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppCore.Services.APIMessages
{

    public class FTMessageClient : FTMessage
    {
        [JsonInclude]
        public string UserGuid;

        public FTMessageClient(string userGuid, string message, string signature) : base(message, signature)
        {
            UserGuid = userGuid;
        }

        public FTMessageClient(string userGuid, string message) : base(message)
        {
            UserGuid = userGuid;
        }

        public FTMessageClient() : base()
        {
            UserGuid = string.Empty;
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static FTMessageClient GenerateNotSecure(string id, string data)
        {
            return new FTMessageClient(id, data);
        }

        public static FTMessageClient GenerateSecure(string id, SecurityManager manager, object data)
        {
            string encrypted = manager.Encrypt(
                JsonSerializer.Serialize(data)
                );
            string signed = manager.SignData(encrypted);
            return new FTMessageClient(id, encrypted, signed);
        }

        public FTMessageClient? FromJson(string json)
        {
            return JsonSerializer.Deserialize<FTMessageClient>(json);
        }
    }
}
