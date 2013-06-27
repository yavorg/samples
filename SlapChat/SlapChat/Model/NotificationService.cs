using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    class NotificationService : INotificationService
    {
        public async void RegisterNotificationHubs(string channel)
        {
            var connectionString = ConnectionString.CreateUsingSharedAccessKey(
                MobileServiceConfig.NotificationHubUri,
                MobileServiceConfig.NotificationHubKey,
                MobileServiceConfig.NotificationHubSecret);
            NotificationHub notificationHub = new NotificationHub("slapchat", connectionString);
            await notificationHub.RegisterNativeAsync(channel, new List<string> { "news" });
        }
        
      
    }
}
