using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPGenerateActivityEngine : EndPointArgs
    {
        public override string Route() => APIRoute.GENERATE_ACTIVITY_ENGINE;
    }
}
