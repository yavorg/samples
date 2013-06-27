using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            NotificationHub notificationHub = new NotificationHub(
                MobileServiceConfig.NotificationHubName, connectionString);
            await notificationHub.UnregisterAllAsync(channel);
            await notificationHub.RegisterTemplateAsync(channel, "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
                "<wp:Toast>" +
                    "<wp:Text1>" + "SlapChat" + "</wp:Text1>" +
                    "<wp:Text2>" + "$(msg)" + "</wp:Text2>" +
                "</wp:Toast> " +
            "</wp:Notification>", "toast");
        }
    }
}
