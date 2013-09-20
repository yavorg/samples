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
            return new BasicGeoposition
            {
                Altitude = g.Coordinate.Altitude ?? 0,
                Latitude = g.Coordinate.Latitude,
                Longitude = g.Coordinate.Longitude                
            };
        }

        
    }
}
