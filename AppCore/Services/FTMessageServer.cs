using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class FTMessageServer : FTMessage
    {
        public FTMessageServer()
        {
        }

        public FTMessageServer(string message) : base(message)
        {
        }

        public FTMessageServer(string message, string signature) : base(message, signature)
        {
        }

        public static FTMessageServer GenerateNotSecure(string data)
        {
            return new FTMessageServer(data);
        }

        public static FTMessageServer SecureSyncKey(SecurityManager manager, string syncKey)
        {
            return new FTMessageServer(Convert.ToBase64String(manager.EncryptUnsafeKey(syncKey)));
        }

        public string ReadSyncKey(SecurityManager manager)
        {
            return manager.DecryptUnsafeKey(Convert.FromBase64String(Message));
        }

        public static FTMessageServer GenerateSecure(SecurityManager manager, object data)
        {
            string encrypted = manager.Encrypt(
                JsonSerializer.Serialize(data)
                );
            string signed = manager.SignData(encrypted);
            return new FTMessageServer(encrypted, signed);
        }

        public FTMessageServer? FromJson(string json)
        {
            return JsonSerializer.Deserialize<FTMessageServer>(json);
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
