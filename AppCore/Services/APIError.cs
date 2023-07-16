using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public static class APIError
    {
        public const string USER_ID_NOT_EXIST = "User id not exist";
        public const string CANCELED_REQUEST = "Canceled request";
        public const string USER_EXPIRED = "User expired";
        public const string QUOTA_EXCEEDED = "Quota exceeded";
        public const string BAD_FORMAT_DATA = "Bad format data";
        public const string BAD_CREDENTIALS = "Bad credentials";
        public const string BAD_ARGS = "Bad args";
        public const string BAD_USER_TOKEN = "Bad user token";
        public const string NOTHING_TO_UPDATE = "Nothing to update";
        public const string VILLAGE_NOT_SET = "Village not set";
        public const string VILLAGE_ALREADY_SET = "Village already set";
        public const string NOT_ENOUGHT_RESSOURCE = "Not enought ressource";
        public const string ERROR_SCHEMA_NOT_FOUND_OR_INCOMPLET = "Error schema not found or incomplet";
        public const string BUILDING_NOT_FOUND = "Building not found";
        public const string BAD_BUILDING_OWNER = "bad building owner";
    }
}
