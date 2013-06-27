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
            FriendsViewModel viewModel = (this.DataContext as FriendsViewModel);
            viewModel.RefreshCommand.Execute(e);
            viewModel.RegisterPushCommand.Execute(e);
        }

    }
}