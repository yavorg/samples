using SlapChat.ViewModel;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace SlapChat.View
{
    public partial class FriendsPage : PhoneApplicationPage
    {
        public FriendsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            (this.DataContext as FriendsViewModel).RefreshCommand.Execute(e);
        }

    }
}