using DotNetRuntimeData.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace DotNetRuntimeData
{
    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<MobileOrder, MobileOrder> orders;
        private MobileServiceCollection<MobileCustomer, MobileCustomer> customers;
        private IMobileServiceSyncTable<MobileOrder> ordersTable = App.MobileService.GetSyncTable<MobileOrder>();
        private IMobileServiceSyncTable<MobileCustomer> customersTable = App.MobileService.GetSyncTable<MobileCustomer>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task Initialize()
        {
            if (!App.MobileService.SyncContext.IsInitialized)
            {
                var store = new MobileServiceSQLiteStore("localdb8.db");
                store.DefineTable<MobileOrder>();
                store.DefineTable<MobileCustomer>();

                await App.MobileService.SyncContext.InitializeAsync(store);
            }

            await RefreshOrderList();
        }

        private async Task RefreshOrderList()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                orders = await ordersTable
                    .OrderByDescending(x => x.Id)
                    .ToCollectionAsync();

                customers = await customersTable.CreateQuery().ToCollectionAsync();

                ListItems.ItemsSource = orders;
                TextCustomer.ItemsSource = customers;
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var customer = TextCustomer.SelectedItem as MobileCustomer;

            if (customer != null)
            {
                var order = new MobileOrder
                {
                    Item = TextOrder.Text,
                    MobileCustomerId = customer.Id,
                    Quantity = 1,
                    MobileCustomerName = customer.Name
                };

                await ordersTable.InsertAsync(order);
                orders.Insert(0, order);

                // clear the UI fields
                TextCustomer.SelectedIndex = -1;
                TextOrder.Text = String.Empty;
                TextOrder.Focus(FocusState.Programmatic);
            }
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            var order = button.DataContext as MobileOrder;
            await ordersTable.UpdateAsync(order);
            button.IsEnabled = false;
        }

        private async void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog d = null;
            ButtonSync.IsEnabled = false;

            try
            {
                await App.MobileService.SyncContext.PushAsync();
                await this.ordersTable.PullAsync();
                await this.customersTable.PullAsync();
            }
            catch (Exception ex)
            {
                d = new MessageDialog(ex.ToString());
            }

            if (d != null)
            {
                await d.ShowAsync();
            }

            await RefreshOrderList();
            ButtonSync.IsEnabled = true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
                await Initialize();
        }

        private void TextOrder_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ButtonAdd.Focus(FocusState.Programmatic);
            }
        }

        private void CheckEnableAdd()
        {
            ButtonAdd.IsEnabled = (TextOrder.Text.Length > 0) && (TextCustomer.SelectedItem != null);
        }

        private void TextCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckEnableAdd();
        }

        private void TextOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckEnableAdd();
        }


        private void TextOrderDetail_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            Button save = (textBox.Parent as Grid).Children.First(x => x is Button) as Button;
            if (!textBox.Text.Equals((textBox.DataContext as MobileOrder).Item))
            {
                save.IsEnabled = true;
            }
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var order = checkbox.DataContext as MobileOrder;

            ordersTable.UpdateAsync(order);
        }
    }
}
