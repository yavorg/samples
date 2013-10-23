using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

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
            this.monitor.Geofences.Clear();
            this.monitor.GeofenceStateChanged += OnGeofenceStateChangedHandler;
            this.monitor.StatusChanged += (sender, args) => Debug.WriteLine(sender.Status.ToString());
            this.TriggerFence = null;
            this.ArmedFences = new List<Geofence>();
            this.Actions = new List<GeofenceLoaderAction>();
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

        private List<GeofenceLoaderAction> loaderActions;
        public List<GeofenceLoaderAction> Actions
        {
            get
            {
                return this.loaderActions;
            }
            private set
            {
                if (value != this.loaderActions)
                {
                    this.loaderActions = value;
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
            var here = pos.Coordinate.Point.Position;
            RefreshTriggerFence(here);
            RefreshArmedFences(here);
        }

        private async void RefreshArmedFences(BasicGeoposition location)
        {
            // Obtain the list of fences
            string partition = "lat" + Math.Floor(location.Latitude * 100).ToString() +
                "lon" + Math.Floor(location.Longitude * 100).ToString();

            var response = await table.Where(f => f.Partition == partition).
                ToListAsync();

            List<Geofence> result = response.Select<ServerGeofence, Geofence>(f =>
            {
                string name = f.Name;
                if (name.Length > 63)
                {
                    name = name.Substring(0, 60);
                }
                else if (name.Length == 0)
                {
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
                    MonitoredGeofenceStates.Exited | MonitoredGeofenceStates.Entered,
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
                catch (Exception e)
                {
                    Debug.WriteLine("Could not add geofence: {0}", e.ToString());
                }
            }

            ArmedFences = result;
        }

        private void OnGeofenceStateChangedHandler(GeofenceMonitor sender, object e)
        {
            var reports = sender.ReadReports();
            Dictionary<string, GeofenceState> reportsBuffer = new Dictionary<string, GeofenceState>();

            foreach (GeofenceStateChangeReport report in reports)
            {
                GeofenceState state = report.NewState;
                Geofence geofence = report.Geofence;
                if (state == GeofenceState.Removed)
                {
                }
                else if (state == GeofenceState.Entered)
                {
                    reportsBuffer.Add(geofence.Id, GeofenceState.Entered);
                }
                else if (state == GeofenceState.Exited)
                {
                    reportsBuffer.Add(geofence.Id, GeofenceState.Exited);
                }
            }

            foreach (KeyValuePair<string, GeofenceState> report in reportsBuffer)
            {
                switch (report.Value)
                {
                    case GeofenceState.Entered:
                        foreach (GeofenceLoaderAction action in Actions)
                        {
                            action.EnterGeofence(report.Key);
                        }
                        break;
                    case GeofenceState.Exited:
                        // They have left the area for which we have fences loaded, 
                        // trigger a refresh
                        if (String.Equals(report.Key, TriggerFence.Id))
                        {
                            BasicGeoposition current = sender.LastKnownGeoposition.Coordinate.Point.Position;
                            RefreshTriggerFence(current);
                            RefreshArmedFences(current);
                        }
                        else
                        {
                            foreach (GeofenceLoaderAction action in Actions)
                            {
                                action.ExitGeofence(report.Key);
                            }
                        }
                        break;
                }
            }
        }

        private void RefreshTriggerFence(BasicGeoposition location)
        {
            // The partition size on the server is a rectangle with sides of 0.01 degree, 
            // so draw a circle to the closest edge of the rectangle           

            double closestLat = Math.Round(location.Latitude, 2);
            double distanceToClosestLat = Math.Abs(closestLat - location.Latitude);
            double closestLon = Math.Round(location.Longitude, 2);
            double distanceToClosestLon = Math.Abs(closestLon - location.Longitude);

            BasicGeoposition closestEdge = new BasicGeoposition();
            if (distanceToClosestLon < distanceToClosestLat)
            {
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
