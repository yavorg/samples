using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.WindowsAzure.Messaging;

namespace SlapChat.Model
{
    public class ChatService : IChatService
    {
        private Dictionary<string, User> users; 
        private Dictionary<string, string> emailAddressToUserId;
        private Dictionary<string, List<string>> friends;
        private List<PhotoRecord> photoRecords;
        private Dictionary<string, PhotoContent> photoContents;
        private Timer timer;

        public ChatService()
	    {
            users = new Dictionary<string, User>();
            emailAddressToUserId = new Dictionary<string, string>();
            friends = new Dictionary<string, List<string>>();
            photoRecords = new List<PhotoRecord>();
            photoContents = new Dictionary<string, PhotoContent>();

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
                            if(photoContents.TryGetValue(p.PhotoContentSecretId, out content)){
                                DeletePhotoContent(content.SecretId);
                            }
                        });
                   
                }),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1));
	    }

        public Task CreateUserAsync(User user)
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

            return Task.FromResult(0);

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

        public Task<ObservableCollection<PhotoRecord>> ReadPhotoRecordsAsync(string userId)
        {
            ObservableCollection<PhotoRecord> results = new ObservableCollection<PhotoRecord>();
            foreach (PhotoRecord p in photoRecords)
            {
                if (String.Equals(p.RecepientUserId, userId) ||
                    String.Equals(p.SenderUserId, userId))
                {
                    results.Add(p);
                }
            }
            return Task.FromResult<ObservableCollection<PhotoRecord>>(results);
        }

        public Task CreatePhotoRecordAsync(PhotoRecord record)
        {
            PhotoContent content = new PhotoContent();
            content.SecretId = Guid.NewGuid().ToString();
            content.Uri = new Uri(String.Format("http://{0}", Guid.NewGuid()));
            photoContents[content.SecretId] = content;
            content.Id = photoContents.Count - 1;

            photoRecords.Add(record);
            record.Id = photoRecords.IndexOf(record);
            record.Sent = DateTimeOffset.Now;
            record.SenderName = App.CurrentUser.Name;
            record.SenderUserId = App.CurrentUser.UserId;
            record.PhotoContentSecretId = content.SecretId;
            record.Expired = false;

            // These two are returned but not stored in the 
            // datastore, we delete them later
            record.UploadKey = String.Empty; // Doesn't matter in this case
            record.Uri = content.Uri;

            content.PhotoRecordId = record.Id;

            return Task.FromResult(0);

        }

        public Task<ObservableCollection<PhotoContent>> ReadPhotoContentAsync(string id)
        {
            PhotoContent content = null;
            if(photoContents.TryGetValue(id, out content))
            {
                PhotoRecord record = photoRecords[content.PhotoRecordId];
                record.Received = DateTimeOffset.Now;
            }

            ObservableCollection<PhotoContent> wrapper = new ObservableCollection<PhotoContent>();
            wrapper.Add(content);
            return Task.FromResult<ObservableCollection<PhotoContent>>(wrapper);

        }

        public void DeletePhotoContent(string id)
        {
            photoContents.Remove(id);
        }

        public Task<HttpResponseMessage> UploadPhotoAsync(Uri location, string secret, Stream photo)
        {
            using(MediaLibrary ml = new MediaLibrary())
            {
                photo.Position = 0;
                ml.SavePictureToCameraRoll(location.ToString(), photo);
            }

            return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage());
        }

        public Stream ReadPhotoAsStream(Uri location)
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

        public Uri ReadPhotoAsUri(Uri location)
        {
            // This method is not supported 
            return null;
        }
    }
}
