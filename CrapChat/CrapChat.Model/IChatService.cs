using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace CrapChat.Model
{
    public interface IChatService
    {
        ObservableCollection<Friend> ReadFriends();
        ObservableCollection<Friend> CreateFriends(IEnumerable<Friend> newFriends);

        ObservableCollection<PhotoRecord> ReadPhotoRecords();
        
        PhotoRecord CreatePhotoRecord(PhotoRecord record);
        
        PhotoContent ReadPhotoContent(Guid id);
        void DeletePhotoContent(Guid id);

        void UploadPhoto(Uri location, Stream photo);
        Stream ReadPhoto(Uri location);
        void DeletePhoto(Uri location);
    }
}
