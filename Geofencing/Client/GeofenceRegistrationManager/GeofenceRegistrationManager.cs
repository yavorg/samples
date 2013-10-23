using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace WindowsAzure
{
    public class GeofenceRegistrationManager : GeofenceLoaderAction
    {
        private MobileServiceClient client;

        private ObservableCollection<Campaign> campaigns = new ObservableCollection<Campaign>();
        public ObservableCollection<Campaign> Campaigns
        {
            get
            {
                return this.campaigns;
            }
        }

        public GeofenceRegistrationManager(Uri applicationUri, string key)
        {
            this.client = new MobileServiceClient(applicationUri, key);
            this.campaigns = new ObservableCollection<Campaign>();
        }

        public override async void EnterGeofence(string name)
        {
            try
            {
                var campaigns = await client.InvokeApiAsync<EnterRequest, List<Campaign>>("entering",
                    new EnterRequest { FenceName = name });
                Campaign it = campaigns.FirstOrDefault();
                if (it != null)
                {
                    Campaigns.Add(it);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public override void ExitGeofence(string name)
        {
            try
            {
                //await client.InvokeApiAsync<List<Campaign>>("leaving", HttpMethod.Post,
                //    new Dictionary<string, string> { { "fenceName", name } });

                Campaigns.Remove(
                    Campaigns.Where(c => String.Equals(c.FenceName, name)).FirstOrDefault());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
