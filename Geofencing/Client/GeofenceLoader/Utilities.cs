using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Windows.Devices
{
    public static class Utilities
    {
        public static BasicGeoposition ToBasicGeoposition(this Geoposition g)
        {
            return g.Coordinate.Point.Position;
        }
    }
}
