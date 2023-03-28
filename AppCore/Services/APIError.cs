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
    }
}
