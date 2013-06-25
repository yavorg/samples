using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SlapChat.ViewModel;

namespace SlapChat.View
{
    public partial class SendPage : PhoneApplicationPage
    {
        public SendPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            (this.DataContext as SendViewModel).RefreshCommand.Execute(e);
        }
    }
}