using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Threading;

namespace CrapChat.Model
{
    public class ChatService : IChatService
    {
        private List<Friend> friends;
        private List<Photo> photos;
        private const string myMicrosoftAccount = "dummy@live.com";
        private const string myName = "Authenticated Dummy";
        private Timer timer;

        public ChatService()
	    {
            friends = new List<Friend>();
            photos = new List<Photo>();

            // Timer to expire any photos that were read more than
            // 30 seconds ago
            timer = new Timer(new TimerCallback((o) =>
                {
                    List<Photo> expired = photos
                        .Where((p) =>
                        {
                            return (p.Received != null) &&  
                            (DateTimeOffset.Now - p.Received > TimeSpan.FromMinutes(30));
                        })
                        .ToList();
                    expired.ForEach((p) => 
                        {
                            p.Expired = true;
                            DeletePhoto(p.Uri);
                        });
                   
                }),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(10));
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
            foreach (Photo p in photos)
            {
                if (p.Expired)
                {
                    p.Uri = null;
                }
                else
                {
                    if (p.Received == null)
                    {
                        p.Received = DateTimeOffset.Now;
                    }
                }
            }
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
            photo.Expired = false;
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
            if (location != null)
            {
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
            }
            return result;
        }

        public void DeletePhoto(Uri location)
        {
            // Not possible to delete photos from MediaLibrary
        }
    }
}
