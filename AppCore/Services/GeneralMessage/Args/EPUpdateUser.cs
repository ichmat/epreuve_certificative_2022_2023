
using System.Text.Json.Serialization;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPUpdateUser : EndPointArgs
    {
        [JsonInclude]
        public string? Mail;
        [JsonInclude]
        public string? Pseudo;
        [JsonInclude]
        public string? MotDePasse;
        [JsonInclude]
        public string? Sel;
        [JsonInclude]
        public float? PoidKg;
        [JsonInclude]
        public ushort? TailleCm;

        public EPUpdateUser(string? mail, string? pseudo, string? motDePasse, string? sel, float? poidKg, ushort? tailleCm)
        {
            Mail = mail;
            Pseudo = pseudo;
            MotDePasse = motDePasse;
            Sel = sel;
            PoidKg = poidKg;
            TailleCm = tailleCm;
        }

        public override string Route() => APIRoute.UPDATE_USER;
    }
}
