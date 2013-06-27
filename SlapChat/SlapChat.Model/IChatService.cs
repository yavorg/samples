using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    public interface IChatService
    {
        Task CreateUserAsync(User user);

        Task<ObservableCollection<User>> ReadFriendsAsync(string userId);
        Task<ObservableCollection<User>> CreateFriendsAsync(string userId, string emailAddresses);

        Task<ObservableCollection<PhotoRecord>> ReadPhotoRecordsAsync(string userId);
        
        Task CreatePhotoRecordAsync(PhotoRecord record);

        Task<ObservableCollection<PhotoContent>> ReadPhotoContentAsync(string id);
        void DeletePhotoContent(string id);

        Task<HttpResponseMessage> UploadPhotoAsync(Uri location, string secret, Stream photo);
        Stream ReadPhoto(Uri location);
    }
}
