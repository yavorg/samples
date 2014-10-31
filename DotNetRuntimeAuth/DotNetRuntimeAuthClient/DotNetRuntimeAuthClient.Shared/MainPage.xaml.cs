using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DotNetRuntimeAuthClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MobileServiceClient MobileService { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            // Make sure you use a service published to Azure, setting up
            // the auth provider redirect flow pointing to localhost 
            // doesnt always work so well.
            MobileService = new MobileServiceClient(
                "https://yavorg-csharpauth.azure-mobile.net", 
                "zQXyKDdRZBdlJsKBVKOXXkzSJxEGqW97");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Authenticated table operations and offline sync will fail

            // Custom auth - LinkedIn provider
            await MobileService.LoginAsync("linkedin");

            MessageDialog dialog = new MessageDialog(MobileService.CurrentUser.UserId, "Login success");
            await dialog.ShowAsync();

            // Authenticated calls will now work

        }
    }
}
