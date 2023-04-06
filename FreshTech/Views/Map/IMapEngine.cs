using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Views
{
    public interface IMapEngine : IDisposable
    {
        public void AddLine(Location location);

        public void CutLine();

        public void UpdateUserLocation(Location location);

        public void MooveScreenTo(Location to, double? zoomMetersAccuracy = null);

        public View GetMapView();
    }
}
