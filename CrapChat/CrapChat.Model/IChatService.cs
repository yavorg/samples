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

        ObservableCollection<Photo> ReadPhotos();
        
        Photo CreatePhoto(Photo photo);
        void UploadPhoto(Uri location, Stream photo);
        Stream ReadPhoto(Uri location);
    }
}
