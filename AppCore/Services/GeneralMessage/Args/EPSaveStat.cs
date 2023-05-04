using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPSaveStat : EndPointArgs
    {
        public override string Route() => APIRoute.SAVE_STAT;

        public Stat Stat { get; set; }

        public EPSaveStat(Stat stat)
        {
            Stat = stat;
        }
    }
}
