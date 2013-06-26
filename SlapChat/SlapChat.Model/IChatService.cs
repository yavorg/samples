using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SlapChat.Model
{
    public interface IChatService
    {
        User CreateUser(User user);

        ObservableCollection<User> ReadFriends(string userId);
        ObservableCollection<User> CreateFriends(string userId, string emailAddresses);

        ObservableCollection<PhotoRecord> ReadPhotoRecords();
        
        PhotoRecord CreatePhotoRecord(PhotoRecord record);
        
        PhotoContent ReadPhotoContent(Guid id);
        void DeletePhotoContent(Guid id);

        void UploadPhoto(Uri location, Stream photo);
        Stream ReadPhoto(Uri location);
        void DeletePhoto(Uri location);
    }
}
