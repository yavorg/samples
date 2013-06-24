using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrapChat.ViewModel;

namespace CrapChat.View
{
    public partial class CameraPage : PhoneApplicationPage
    {
        public CameraPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            (this.DataContext as CameraViewModel).SetUpCameraCommand.Execute(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            (this.DataContext as CameraViewModel).CleanUpCameraCommand.Execute(e);
        }
    }
}