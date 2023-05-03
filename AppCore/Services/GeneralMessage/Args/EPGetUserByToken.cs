using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPGetUserByToken : EndPointArgs
    {
        public EPGetUserByToken() : base() { }

        public override string Route() => APIRoute.GET_USER_BY_TOKEN;

    }
}
