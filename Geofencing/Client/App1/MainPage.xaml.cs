using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsAzure;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Pushpin currentLocationPushpin;
        private GeofenceLoader loader;
        private GeofenceRegistrationManager registrationManager;
        private Geolocator locator;

        public MainPage()
        {
            this.InitializeComponent();
            this.myMap.Credentials = MobileSecrets.BingMapCredentials;

            currentLocationPushpin = new Pushpin();
            currentLocationPushpin.Background = new SolidColorBrush(Colors.Black);

            myMap.Children.Add(currentLocationPushpin);

            locator = new Geolocator();
            locator.MovementThreshold = 10;
            locator.PositionChanged += locator_PositionChanged;

            // Set up geofence loader
            this.loader = new GeofenceLoader(
                new Uri(MobileSecrets.MobileServiceUrl),
                MobileSecrets.MobileServiceKey
                );
            loader.PropertyChanged += loader_PropertyChanged;

            // Set up geofence registration manager       
            this.registrationManager = new GeofenceRegistrationManager(
                new Uri(MobileSecrets.MobileServiceUrl),
                MobileSecrets.MobileServiceKey);
            registrationManager.Campaigns.CollectionChanged += Campaigns_CollectionChanged;

            loader.Actions.Add(registrationManager);
        }

        async void Campaigns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Campaign c = registrationManager.Campaigns.FirstOrDefault();
                if (c != null)
                {
                    offerUrl.NavigateUri = c.Url;
                    offerUrl.Content = "Redeem";
                }
                else
                {
                    offerUrl.NavigateUri = null;
                    offerUrl.Content = String.Empty;
                }
            });
        }

     

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Apparently first call needs to happen on UI thread
            await locator.GetGeopositionAsync();
            loader.Start();
        }

        async void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Location start = args.Position.ToLocation();
                MapLayer.SetPosition(currentLocationPushpin, start);
                myMap.SetView(start, 15);
            });
        }

        async void loader_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.Equals(e.PropertyName, "ArmedFences") || String.Equals(e.PropertyName, "TriggerFence"))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    List<Geofence> result = loader.ArmedFences;

                    // Add application fences
                    MapShapeLayer fences = new MapShapeLayer();
                    MapLayer pushpins = new MapLayer();
                    foreach (Geofence f in result)
                    {
                        Geocircle c1 = f.Geoshape as Geocircle;
                        MapPolyline fence = new MapPolyline();
                        fence.Color = Colors.Red;
                        fence.Width = 5;
                        fence.Locations = DrawMapsCircle(c1.Center, c1.Radius);
                        fences.Shapes.Add(fence);

                        Pushpin p = new Pushpin();
                        p.Background = new SolidColorBrush(Colors.Red);
                        ToolTipService.SetToolTip(p, f.Id);
                        MapLayer.SetPosition(p, new Location(c1.Center.Latitude, c1.Center.Longitude));
                        pushpins.Children.Add(p);
                    }

                    // Add trigger fence
                    Geocircle c2 = loader.TriggerFence.Geoshape as Geocircle;
                    MapPolyline trigger = new MapPolyline();
                    trigger.Color = Colors.Gray;
                    trigger.Width = 5;
                    trigger.Locations = DrawMapsCircle(c2.Center, c2.Radius);
                    fences.Shapes.Add(trigger);

                    myMap.ShapeLayers.Clear();
                    myMap.ShapeLayers.Add(fences);

                    // Clear existing pushpins
                    var existingLayers = myMap.Children.Where(c => c is MapLayer).ToArray();
                    for (int i = 0; i < existingLayers.Count(); i++)
                    {
                        myMap.Children.Remove(existingLayers[i]);
                    }
                    myMap.Children.Add(pushpins);
                });
            }
        }

        private static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        private static double ToRadian(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static LocationCollection DrawMapsCircle(BasicGeoposition location, double radius)
        {
            LocationCollection destination = new LocationCollection();
            double earthRadiusInMeters = 6367.0 * 1000.0;
            var lat = ToRadian(location.Latitude);
            var lng = ToRadian(location.Longitude);
            var d = radius / earthRadiusInMeters;


            for (var x = 0; x <= 360; x++)
            {
                var brng = ToRadian(x);
                var latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                var lngRadians = lng + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

                destination.Add(new Location()
                {
                    Latitude = ToDegrees(latRadians),
                    Longitude = ToDegrees(lngRadians)
                });
            }

            return destination;
        }

        private void GoryDetailsClick(object sender, RoutedEventArgs e)
        {
            mapView.Visibility = Visibility.Visible;
            homeView.Visibility = Visibility.Collapsed;
        }

        private void HappyPlaceClick(object sender, RoutedEventArgs e)
        {
            mapView.Visibility = Visibility.Collapsed;
            homeView.Visibility = Visibility.Visible;
        }

    }
}
