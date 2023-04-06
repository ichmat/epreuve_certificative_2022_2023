using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Views
{
    public interface IMapEngine
    {

        public void DrawLine(Location from, Location to);

        public void MooveTo(Location to);

        public IView GetMapView();

        public enum LocalisationError
        {
            None = 0,
            NotSupported = 1,
            NotEnabled = 2,
            NeedPermission = 3,
            Unknown = 4
        }
    }
}
