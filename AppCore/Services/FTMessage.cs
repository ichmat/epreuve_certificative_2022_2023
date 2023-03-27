using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppCore.Services
{
    public abstract class FTMessage
    {
        public const string EMPTY_SIGNATURE = "empty signature";
        public const string BAD_SIGNATURE = "bad signature";
        public const string UNABLE_TO_DECRYPT = "unable to decrypt";

        [JsonInclude]
        public string Message;
        [JsonInclude]
        public string? Signature;

        public FTMessage(string message, string signature)
        {
            Message = message;
            Signature = signature;
        }

        public FTMessage(string message)
        {
            Message = message;
        }

        public FTMessage()
        {
            Message = string.Empty;
            Signature = null;
        }
        

        public abstract string ToJson();

        public T? SecureDecrypt<T>(SecurityManager manager) where T : class 
        {
            if (Signature == null) throw new Exception(EMPTY_SIGNATURE);

            if (!manager.CheckSign(Message, Signature))
            {
                throw new Exception(BAD_SIGNATURE);
            }

            return JsonSerializer.Deserialize<T>(manager.Decrypt(Message));
        }
    }
}
