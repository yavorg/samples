using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Threading;

namespace SlapChat.Model
{
    public class ChatService : IChatService
    {
        private List<Friend> friends;
        private List<PhotoRecord> photoRecords;
        private Dictionary<Guid, PhotoContent> photoContents;
        private Timer timer;

        public ChatService()
	    {
            friends = new List<Friend>();
            photoRecords = new List<PhotoRecord>();
            photoContents = new Dictionary<Guid, PhotoContent>();

            // Timer to expire any photos that were read more than
            // 30 seconds ago
            timer = new Timer(new TimerCallback((o) =>
                {
                    List<PhotoRecord> expired = photoRecords
                        .Where((p) =>
                        {
                            return (p.Received != new DateTimeOffset()) &&  
                            (DateTimeOffset.Now - p.Received > TimeSpan.FromSeconds(5));
                        })
                        .ToList();
                    expired.ForEach((p) => 
                        {
                            p.Expired = true;

                            PhotoContent content = null;
                            if(photoContents.TryGetValue(p.PhotoContentId, out content)){
                                DeletePhoto(content.Uri);
                                DeletePhotoContent(content.Id);
                            }
                        });
                   
                }),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1));
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


        public ObservableCollection<PhotoRecord> ReadPhotoRecords()
        {
            ObservableCollection<PhotoRecord> results = new ObservableCollection<PhotoRecord>();
            foreach (PhotoRecord p in photoRecords)
            {
                if (String.Equals(p.RecepientMicrosoftAccount, App.CurrentUser.MicrosoftAccount) ||
                    String.Equals(p.SenderMicrosoftAccount, App.CurrentUser.MicrosoftAccount))
                {
                    results.Add(p);
                }
            }
            return results;
        }

        public PhotoRecord CreatePhotoRecord(PhotoRecord record)
        {
            PhotoContent content = new PhotoContent();
            content.Id = Guid.NewGuid();
            content.Uri = new Uri(String.Format("http://{0}", Guid.NewGuid()));
            photoContents[content.Id] = content;

            photoRecords.Add(record);
            record.Id = photoRecords.IndexOf(record);
            record.Sent = DateTimeOffset.Now;
            record.SenderName = App.CurrentUser.Name;
            record.SenderMicrosoftAccount = App.CurrentUser.MicrosoftAccount;
            record.PhotoContentId = content.Id;
            record.Expired = false;

            content.PhotoRecordId = record.Id;

            return record;
        }

        public PhotoContent ReadPhotoContent(Guid id)
        {
            PhotoContent content = null;
            if(photoContents.TryGetValue(id, out content))
            {
                if (content.Uploaded == true)
                {
                    PhotoRecord record = photoRecords[content.PhotoRecordId];
                    record.Received = DateTimeOffset.Now;
                }
                else
                {
                    content.Uploaded = true;
                }
            }

            return content;

        }

        public void DeletePhotoContent(Guid id)
        {
            photoContents.Remove(id);
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
