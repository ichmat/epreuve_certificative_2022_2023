using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage
{
    public sealed class Credentials
    {
        [JsonInclude]
        public string? User;
        [JsonInclude]
        public string Password;
        [JsonInclude]
        public string? Email;

        public Credentials() { Password = String.Empty; }

        public Credentials(string? user, string password, string? email)
        {
            User = user;
            Password = password;
            Email = email;
        }
    }
}
