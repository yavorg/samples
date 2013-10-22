using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation.Geofencing;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using Windows.System;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace WindowsAzure
{
    public class GeofenceRegistrationManager : GeofenceLoaderAction, INotifyPropertyChanged
    {
        private MobileServiceClient client;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Campaign> campaigns;
        public ObservableCollection<Campaign> Campaigns
        {
            get
            {
                return this.campaigns;
            }
            private set
            {
                if (value != this.campaigns)
                {
                    this.campaigns = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public GeofenceRegistrationManager(Uri applicationUri, string key)
        {
            this.client = new MobileServiceClient(applicationUri, key);
            this.campaigns = new ObservableCollection<Campaign>();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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
            catch (Exception e2)
            {
            }
        }

        public override void ExitGeofence(string name)
        {
            try { 
                                
                /*
                await client.InvokeApiAsync<List<Campaign>>("leaving", HttpMethod.Post,
                    new Dictionary<string, string> { { "fenceName", name } });
                */

                Campaigns.Remove(
                    Campaigns.Where(c => String.Equals(c.FenceName, name)).FirstOrDefault());

                                 
            }
            catch (Exception e1)
            {
                                
            }
        }
    }
}
