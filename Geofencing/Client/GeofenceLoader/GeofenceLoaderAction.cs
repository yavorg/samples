using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzure
{
    public abstract class GeofenceLoaderAction
    {
        public GeofenceLoaderAction()
        {

        }

        public abstract void EnterGeofence(string name);

        public abstract void ExitGeofence(string name);

    }
}
