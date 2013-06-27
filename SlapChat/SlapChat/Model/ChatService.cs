using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    public class ChatService : IChatService
    {
        private Dictionary<string, User> users; 
        private Dictionary<string, string> emailAddressToUserId;
        private Dictionary<string, List<string>> friends;
        private List<PhotoRecord> photoRecords;
        private Dictionary<Guid, PhotoContent> photoContents;
        private Timer timer;

        public ChatService()
	    {
            users = new Dictionary<string, User>();
            emailAddressToUserId = new Dictionary<string, string>();
            friends = new Dictionary<string, List<string>>();
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

        public void CreateUserAsync(User user)
        {
            if (!users.ContainsKey(user.UserId))
            {
                users[user.UserId] = user;
                user.Id = users.Count - 1;
            }
            else
            {
                user.Id = users[user.UserId].Id;
                users[user.UserId].Name = user.Name;
                users[user.UserId].MpnsChannel = user.MpnsChannel;

            }

            foreach (string email in user.EmailAddresses.Split(' '))
            {
                emailAddressToUserId[email] = user.UserId;
            }

        }

        public Task<ObservableCollection<User>> ReadFriendsAsync(string userId)
        {
            ObservableCollection<User> result = new ObservableCollection<User>();
            List<string> friendIds = null;

            // Get the array of friends for this user
            if(friends.ContainsKey(userId)){
                friendIds = friends[userId];
            }

            if (friendIds != null && friendIds.Count != 0)
            {
                // Return all the friends
                foreach (string id in friendIds)
                {
                    result.Add(users[id]);
                }

            }
            return Task.FromResult<ObservableCollection<User>>(result);
        }

        public Task<ObservableCollection<User>> CreateFriendsAsync(string userId, string emailAddresses)
        {
            ObservableCollection<User> result = new ObservableCollection<User>();
            foreach (string email in emailAddresses.Split(' '))
            {
                // We have a match for that email address
                if (emailAddressToUserId.ContainsKey(email))
                {
                    User friend = users[emailAddressToUserId[email]];
                    
                    // Make sure we don't add ourselves as our friend 
                    if (!String.Equals(friend.UserId, userId))
                    {
                        if (!friends.ContainsKey(userId) || friends[userId] == null)
                        {
                            friends[userId] = new List<string>();
                        }

                        // Ensure no duplicates                    
                        if (!friends[userId].Contains(friend.UserId))
                        {
                            friends[userId].Add(friend.UserId);
                        }

                        result.Add(friend);
                    }
                }
            }

            return Task.FromResult<ObservableCollection<User>>(result);
        }

        public ObservableCollection<PhotoRecord> ReadPhotoRecords()
        {
            ObservableCollection<PhotoRecord> results = new ObservableCollection<PhotoRecord>();
            foreach (PhotoRecord p in photoRecords)
            {
                if (String.Equals(p.RecepientMicrosoftAccount, App.CurrentUser.UserId) ||
                    String.Equals(p.SenderMicrosoftAccount, App.CurrentUser.UserId))
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
            record.SenderMicrosoftAccount = App.CurrentUser.UserId;
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
