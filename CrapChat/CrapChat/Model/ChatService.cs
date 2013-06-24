using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Media;
using System.Linq;

namespace CrapChat.Model
{
    public class ChatService : IChatService
    {
        private List<Friend> friends;
        private List<Photo> photos;
        private const string myMicrosoftAccount = "dummy@live.com";
        private const string myName = "Authenticated Dummy";

        public ChatService()
	    {
            friends = new List<Friend>();
            photos = new List<Photo>();
	    }

        public ObservableCollection<Friend> ReadFriends()
        {
            return new ObservableCollection<Friend>(friends);
        }

        public ObservableCollection<Friend> CreateFriends(IEnumerable<Friend> newFriends)
        {
            foreach (Friend f in newFriends)
            {
                friends.Add(f);
                f.Id = friends.IndexOf(f);
            }
            return new ObservableCollection<Friend>(newFriends);
        }


        public ObservableCollection<Photo> ReadPhotos()
        {
            return new ObservableCollection<Photo>(photos);
        }

        public Photo CreatePhoto(Photo photo)
        {
            photos.Add(photo);
            photo.Id = photos.IndexOf(photo);
            photo.Sent = DateTimeOffset.Now;
            photo.SenderName = myName;
            photo.SenderMicrosoftAccount = myMicrosoftAccount;
            photo.Uri = new Uri(String.Format("http://{0}", Guid.NewGuid()));
            return photo;
        }

        public void UploadPhoto(Uri location, Stream photo)
        {
            using(MediaLibrary ml = new MediaLibrary())
            {
                photo.Position = 0;
                ml.SavePictureToCameraRoll(location.ToString(), photo);
            }
        }

        public Stream ReadPhoto(Uri location)
        {
            Stream result = null;
            using (MediaLibrary ml = new MediaLibrary())
            {
                PictureAlbum cameraRoll = ml.RootPictureAlbum.Albums
                    .Where((a) => String.Equals(a.Name, "Camera Roll"))
                    .FirstOrDefault();
                if (cameraRoll != null)
                {
                    Picture match = cameraRoll.Pictures
                        .Where((p) => String.Equals(p.Name, location.ToString()))
                        .FirstOrDefault();
                    if (match != null)
                    {
                        result = match.GetImage();
                    }       
                }

            }
            return result;
        }
    }
}
