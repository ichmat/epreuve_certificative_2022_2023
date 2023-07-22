using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPCreateUserVillage : EndPointArgs
    {
        public override string Route() => APIRoute.CREATE_USER_VILLAGE;
    }
}
