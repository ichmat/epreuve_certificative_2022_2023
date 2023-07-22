using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Views.Map
{
    internal struct MapPoint
    {
        public double Latitude;
        public double Longitude;
        public DateTime Date;

        public MapPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Date = DateTime.Now;
        }

        public MapPoint(Location location) : this(location.Latitude, location.Longitude) { }
    }
}
