using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPGetNecessaryDataVillage : EndPointArgs
    {
        public override string Route() => APIRoute.GET_NECESSARY_DATA_VILLAGE;
    }
}
