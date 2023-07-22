using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPCreateUser : EndPointArgs
    {
        [JsonInclude]
        public string Mail;
        [JsonInclude]
        public string Pseudo;
        [JsonInclude]
        public string MotDePasse;
        [JsonInclude]
        public string Sel;
        [JsonInclude]
        public float? PoidKg;
        [JsonInclude]
        public ushort? TailleCm;

        public EPCreateUser(string mail, string pseudo, string motDePasse, string sel, float? poidKg, ushort? tailleCm)
        {
            Mail = mail;
            Pseudo = pseudo;
            MotDePasse = motDePasse;
            Sel = sel;
            PoidKg = poidKg;
            TailleCm = tailleCm;
        }

        public override string Route() => APIRoute.CREATE_USER;
    }
}
