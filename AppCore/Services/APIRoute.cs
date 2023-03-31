using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public static class APIRoute
    {
        #region CONNEXION_CONTROLLER
        public const string ESTABLISH_CONNECTION = "/connect/establish";
        public const string SIGN_KEY = "/connect/sign";
        public const string RESET_TIME_OUT = "/connect/resettimeout";
        public const string ATTEMPT_CONNECTION = "/connect/attempt";
        #endregion
        #region UTILISATEUR_CONTROLLER
        public const string CREATE_USER = "/user/create";
        #endregion
    }
}
