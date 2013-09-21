using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Messaging;
using Windows.Devices.Geolocation.Geofencing;


namespace WindowsAzure
{
    public class GeofenceRegistrationManager
    {
        private GeofenceMonitor monitor;
        private NotificationHub hub;
        private string pushChannel;
        private List<string> tags;
        private const string triggerFenceName = "WindowsAzure.TriggerFence";

        public GeofenceRegistrationManager(string hubName, string hubConnectionString, string pushChannel)
        {
            this.tags = new List<string>();
            this.hub = new NotificationHub(hubName, hubConnectionString);
            this.pushChannel = pushChannel;
            this.monitor = GeofenceMonitor.Current;
            this.monitor.GeofenceStateChanged += OnGeofenceStateChangedHandler;    
        }

        private Registration GenerateRegistration()
        {
            string[] sanitizedTags = tags.ToArray();
            for (int i = 0; i < sanitizedTags.Length; i++)
            {
                sanitizedTags[i] = sanitizedTags[i].Replace(" ", "_");
            }

            return new Registration(this.pushChannel, sanitizedTags);
        }

        public async void OnGeofenceStateChangedHandler(GeofenceMonitor sender, object e)
        {
            bool tagsChanged = false;
            var reports = sender.ReadReports();

          
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;

                    Geofence geofence = report.Geofence;

                    if (!String.Equals(geofence.Id, triggerFenceName))
                    {

                        if (state == GeofenceState.Removed)
                        {


                        }
                        else if (state == GeofenceState.Entered)
                        {
                            tags.Add(geofence.Id);
                            tagsChanged = true;
                        }
                        else if (state == GeofenceState.Exited)
                        {
                            tags.RemoveAll(x => String.Equals(x, geofence.Id));
                            tagsChanged = true;
                        }
                    }
                }

                if (tagsChanged)
                {
                    await hub.RegisterAsync(GenerateRegistration());
                }
            
        }
    }
}
