using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPGetStatByUserId : EndPointArgs
    {
        public override string Route() => APIRoute.GET_STAT_BY_USER_ID;

        public EPGetStatByUserId()
        {
        }
    }
}
