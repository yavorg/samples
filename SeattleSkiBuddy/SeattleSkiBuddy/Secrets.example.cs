using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Xaml;

namespace SeattleSkiBuddy
{
    partial class App : Application
    {
        partial void AddClientSecrets(ref MobileServiceClient client)
        {
            client = new MobileServiceClient(   
                "{ Mobile Service URL }",
                "{ Mobile Service key }"
            );
        }
    }
}
