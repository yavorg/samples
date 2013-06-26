using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlapChat.Model
{
    class ConnectedChatService : IChatService
    {
        private MobileServiceClient client;
        private IMobileServiceTable<User> friends;

        public ConnectedChatService()
        {
            client = new MobileServiceClient(MobileServiceConfig.ApplicationUri,
                MobileServiceConfig.ApplicationKey);
            friends = client.GetTable<User>();
        }

        public System.Collections.ObjectModel.ObservableCollection<User> ReadFriends()
        {
            
        }

        public System.Collections.ObjectModel.ObservableCollection<User> CreateFriends(IEnumerable<User> newFriends)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ObservableCollection<PhotoRecord> ReadPhotoRecords()
        {
            throw new NotImplementedException();
        }

        public PhotoRecord CreatePhotoRecord(PhotoRecord record)
        {
            throw new NotImplementedException();
        }

        public PhotoContent ReadPhotoContent(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeletePhotoContent(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UploadPhoto(Uri location, System.IO.Stream photo)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream ReadPhoto(Uri location)
        {
            throw new NotImplementedException();
        }

        public void DeletePhoto(Uri location)
        {
            throw new NotImplementedException();
        }
    }
}
