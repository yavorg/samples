using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using System.IO;
using Newtonsoft.Json.Linq;
using Windows.Devices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAzure.MobileServices;

namespace WindowsAzure
{
    public class GeofenceLoader : INotifyPropertyChanged
    {
        private MobileServiceClient client;
        private GeofenceMonitor monitor;
        private IMobileServiceTable<ServerGeofence> table;


        public event PropertyChangedEventHandler PropertyChanged;

        public GeofenceLoader(Uri applicationUri, string key)
        {
            this.client = new MobileServiceClient(applicationUri, key);
            this.table = client.GetTable<ServerGeofence>();
            this.monitor = GeofenceMonitor.Current;
            this.monitor.GeofenceStateChanged += OnGeofenceStateChangedHandler;       
            this.TriggerFence = null;
            this.ArmedFences = new List<Geofence>();

        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Geofence triggerFence;
        public Geofence TriggerFence
        {
            get
            {
                return this.triggerFence;
            }
            private set
            {
                if (value != this.triggerFence)
                {
                    this.triggerFence = value;
                    NotifyPropertyChanged();
                }

            }
        }

        private List<Geofence> armedFences;
        public List<Geofence> ArmedFences
        {
            get
            {
                return this.armedFences;
            }
            private set
            {
                if (value != this.armedFences)
                {
                    this.armedFences = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public async void Start()
        {
            Geolocator g = new Geolocator();
            Geoposition pos = await g.GetGeopositionAsync();
            var here = (pos).ToBasicGeoposition();
            RefreshTriggerFence(here);
            RefreshArmedFences(here);
        }

        
        public async void RefreshArmedFences(BasicGeoposition location)
        {
            // Obtain the list of fences
            string partition = Math.Floor(location.Latitude * 100).ToString() + 
                Math.Floor(location.Longitude * 100).ToString();

            var response = await table.Where(f => f.Partition == partition).
                ToListAsync();

            List<Geofence> result = response.Select<ServerGeofence, Geofence>(f => {
                string name = f.FenceId;
                if(name.Length > 63){
                    name = name.Substring(0, 60) + "...";
                } else if (name.Length == 0){
                    name = "Unknown";
                }

                return new Geofence(
                    name,
                    new Geocircle(
                        new BasicGeoposition
                        {
                            Altitude = 0,
                            Latitude = f.Lat,
                            Longitude = f.Lon
                        },
                        100),
                    MonitoredGeofenceStates.Exited|MonitoredGeofenceStates.Entered,
                    false,
                    TimeSpan.FromSeconds(1));
            }).ToList<Geofence>();
               

            // Arm them in the geofence monitor
            monitor.Geofences.Clear();
            monitor.Geofences.Add(TriggerFence);
            foreach (Geofence f in result)
            {
                try
                {
                    monitor.Geofences.Add(f);
                }
                catch (Exception) {
                    // Could not add fence, silently ignore
                }
            }

            ArmedFences = result;
            
        }

      

        public void OnGeofenceStateChangedHandler(GeofenceMonitor sender, object e)
        {
            var reports = sender.ReadReports();

            foreach (GeofenceStateChangeReport report in reports)
            {
                GeofenceState state = report.NewState;

                Geofence geofence = report.Geofence;

                
                if (state == GeofenceState.Removed)
                {
                   
                    
                }
                else if (state == GeofenceState.Entered)
                {
                }
                else if (state == GeofenceState.Exited)
                {
                    if(String.Equals(geofence.Id, TriggerFence.Id)){
                        // They have left the area for which we have fences loaded, 
                        // trigger a reload
                        BasicGeoposition current = sender.LastKnownGeoposition.ToBasicGeoposition();
                        RefreshTriggerFence(current);
                        RefreshArmedFences(current);
                    }
                }
            }
        }

        public void RefreshTriggerFence(BasicGeoposition location)
        {
            // The partition size on the server is a rectangle with sides of 0.01 degree, 
            // so draw a circle to the closest edge of the rectangle           
           
            double closestLat = Math.Round(location.Latitude, 2);
            double distanceToClosestLat = Math.Abs(closestLat - location.Latitude);
            double closestLon = Math.Round(location.Longitude, 2);
            double distanceToClosestLon =  Math.Abs(closestLon - location.Longitude);

            BasicGeoposition closestEdge = new BasicGeoposition();
            if(distanceToClosestLon < distanceToClosestLat){
                closestEdge.Latitude = location.Latitude;
                closestEdge.Longitude = closestLon;
            }
            else
            {
                closestEdge.Longitude = location.Longitude;
                closestEdge.Latitude = closestLat;
            }

            var distance = Distance(location, closestEdge);
            // Disallow distance of 0 meters, default to 1 meter
            if (distance == 0)
            {
                distance = 1;
            }

            TriggerFence = new Geofence(
                    "WindowsAzure.TriggerFence",
                    new Geocircle(
                        new BasicGeoposition
                        {
                            Altitude = 0,
                            Latitude = location.Latitude,
                            Longitude = location.Longitude
                        },
                        distance
                    ), 
                    MonitoredGeofenceStates.Exited,
                    false,
                    TimeSpan.FromSeconds(1));
        }

        private double Distance(BasicGeoposition from, BasicGeoposition to)
        {
            double dLon = ToRadian(to.Longitude - from.Longitude);
            double dLat = ToRadian(to.Latitude - from.Latitude);

            double a = Math.Pow(Math.Sin(dLat / 2.0), 2) +
                    Math.Cos(ToRadian(from.Latitude)) *
                    Math.Cos(ToRadian(to.Latitude)) *
                    Math.Pow(Math.Sin(dLon / 2.0), 2.0);

            double c = 2 * Math.Asin(Math.Min(1.0, Math.Sqrt(a)));
            double d = 6367.0 * 1000.0 * c;

            return d;
        }

        private static double ToRadian(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

    }
}
