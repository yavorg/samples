using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Devices.Geolocation
{
    public static class Utilities
    {
        public static Location ToLocation(this Geoposition g)
        {
            return new Location(g.Coordinate.Point.Position.Latitude, g.Coordinate.Point.Position.Longitude);
        }
      
    }
}
