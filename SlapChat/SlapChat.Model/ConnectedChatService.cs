using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http;

namespace SlapChat.Model
{
    public class ConnectedChatService : IChatService
    {
        private MobileServiceClient client;
        private IMobileServiceTable<User> usersTable;
        private IMobileServiceTable<PhotoContent> contentsTable;
        private IMobileServiceTable<PhotoRecord> recordsTable;

        public ConnectedChatService()
        {
            client = new MobileServiceClient(MobileServiceConfig.ApplicationUri,
                MobileServiceConfig.ApplicationKey);
            usersTable = client.GetTable<User>();
            contentsTable = client.GetTable<PhotoContent>();
            recordsTable = client.GetTable<PhotoRecord>();
        }

        public async Task CreateUserAsync(User user)
        {
            await usersTable.InsertAsync(user);
        }


        public async Task<ObservableCollection<User>> ReadFriendsAsync(string userId)
        {
            return await client.InvokeApiAsync<ObservableCollection<User>>("readfriends",
                HttpMethod.Get,
                new Dictionary<string, string>
                {
                    {"userId", userId}
                });
        }

        public async Task<ObservableCollection<User>> CreateFriendsAsync(string userId, string emailAddresses)
        {
            return await client.InvokeApiAsync<ObservableCollection<User>>("createfriends",
                HttpMethod.Post,
                new Dictionary<string, string>
                {
                    {"emailAddresses", emailAddresses},
                    {"userId", userId}
                });
        }

        public async Task<ObservableCollection<PhotoRecord>> ReadPhotoRecordsAsync(string userId)
        {
            return await recordsTable.WithParameters(new Dictionary<string, string>
            {
                {"userId", userId}
            }).ToCollectionAsync<PhotoRecord>();
        }

        public async Task CreatePhotoRecordAsync(PhotoRecord record)
        {
            await recordsTable.InsertAsync(record);
        }

        public PhotoContent ReadPhotoContent(string id)
        {
            throw new NotImplementedException();
        }

        public void DeletePhotoContent(string id)
        {
            //throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> UploadPhotoAsync(Uri location, string secret, System.IO.Stream photo)
        {
            using (var client = new HttpClient())
            {
                UriBuilder uri = new UriBuilder(location);
                uri.Query = secret;

                photo.Position = 0;
                var content = new StreamContent(photo);
                content.Headers.Add("x-ms-blob-type", "BlockBlob");
                content.Headers.Add("Content-Type", "image/jpeg");
                return await client.PutAsync(uri.Uri, content);
            }
        }

        public System.IO.Stream ReadPhoto(Uri location)
        {
            throw new NotImplementedException();
        }

        public void DeletePhoto(Uri location)
        {
            //throw new NotImplementedException();
        }
    }
}
