using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    public interface IChatService
    {
        void CreateUserAsync(User user);

        Task<ObservableCollection<User>> ReadFriendsAsync(string userId);
        Task<ObservableCollection<User>> CreateFriendsAsync(string userId, string emailAddresses);

        ObservableCollection<PhotoRecord> ReadPhotoRecords();
        
        PhotoRecord CreatePhotoRecord(PhotoRecord record);
        
        PhotoContent ReadPhotoContent(Guid id);
        void DeletePhotoContent(Guid id);

        void UploadPhoto(Uri location, Stream photo);
        Stream ReadPhoto(Uri location);
        void DeletePhoto(Uri location);
    }
}
