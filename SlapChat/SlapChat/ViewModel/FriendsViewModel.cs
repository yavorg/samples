using SlapChat.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.UserData;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Diagnostics;
using Microsoft.Phone.Notification;
using Microsoft.WindowsAzure.Messaging;

namespace SlapChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        private IChatService chatService;
        private INotificationService notificationService;

        public FriendsViewModel()
        {
            chatService = ServiceLocator.Current.GetInstance<IChatService>();
            notificationService = ServiceLocator.Current.GetInstance<INotificationService>();

            Contacts contacts = new Contacts();
            contacts.SearchCompleted += contacts_SearchCompleted;
            contacts.SearchAsync(String.Empty, FilterKind.None, null);

            InviteContacts = new RelayCommand(() =>
            {
                if (CurrentUser != null)
                {
                    StringBuilder emailAddresses = new StringBuilder();
                    foreach (User contact in Contacts)
                    {
                        emailAddresses.Append(contact.EmailAddresses).Append(" ");
                    }
                    emailAddresses.Remove(emailAddresses.Length - 1, 1);

                    chatService.CreateFriendsAsync(CurrentUser.UserId, 
                        emailAddresses.ToString());

                    ReadFriends();
                }
            });


            RegisterPushCommand = new RelayCommand(() =>
            {
                /// Holds the push channel that is created or found.
                HttpNotificationChannel pushChannel;

                // The name of our push channel.
                string channelName = "slapchat";

                // Try to find the push channel.
                pushChannel = HttpNotificationChannel.Find(channelName);

                // If the channel was not found, then create a new connection to the push service.
                if (pushChannel == null)
                {
                    pushChannel = new HttpNotificationChannel(channelName);

                    // Register for all the events before attempting to open the channel.
                    pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                    pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                    pushChannel.Open();

                    // Bind this new channel for Tile events.
                    pushChannel.BindToShellToast();

                }
                else
                {
                    // The channel was already open, so just register for all the events.
                    pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                    pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                    Debug.WriteLine(pushChannel.ChannelUri.ToString());
                    PushChannel = pushChannel.ChannelUri;

                }

            });

            RefreshCommand = new RelayCommand(() =>
            {
                ReadFriends();
            });

            PropertyChanged += FriendsViewModel_PropertyChanged;

        }

        public const string PushChannelPropertyName = "PushChannel";
        private Uri pushChannel;

        public Uri PushChannel
        {
            get
            {
                return pushChannel;
            }

            set
            {
                if (pushChannel == value)
                {
                    return;
                }

                pushChannel = value;
                RaisePropertyChanged(PushChannelPropertyName);
            }
        }

        public const string ContactsPropertyName = "Contacts";
        private List<User> contacts;

        public List<User> Contacts
         {
             get
             {
                return contacts;
             }

             set
             {
                if (contacts == value)
                 {
                     return;
                 }

                contacts = value;
                RaisePropertyChanged(ContactsPropertyName);
             }
         }



        public const string CurrentUserPropertyName = "CurrentUser";
        private User currentUser;

        public User CurrentUser
        {
            get
            {
                return currentUser;
            }

            set
            {
                if (currentUser == value)
                {
                    return;
                }

                currentUser = value;
                RaisePropertyChanged(CurrentUserPropertyName);
            }
        }

        public RelayCommand InviteContacts
        {
            get;
            private set;
        }

        public RelayCommand RefreshCommand
        {
            get;
            private set;
        }

        public RelayCommand RegisterPushCommand
        {
            get;
            private set;
        }

        public const string FriendsPropertyName = "Friends";
        public ObservableCollection<User> friends;
        public ObservableCollection<User> Friends
        {
            get
            {
                return friends;
            }

            private set
            {
                if (friends == value)
                {
                    return;
                }

                friends = value;
                RaisePropertyChanged(FriendsPropertyName);
            }
        }


        void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            Contacts = e.Results.Select<Contact, User>((c) =>
                {
                    StringBuilder emailAddresses = new StringBuilder();
                    if (c.EmailAddresses.Count() != 0)
                    {
                        foreach (ContactEmailAddress a in c.EmailAddresses)
                        {
                            emailAddresses.Append(a.EmailAddress).Append(" ");
                        }
                        emailAddresses.Remove(emailAddresses.Length - 1, 1);
                    }

                    return new User
                    {
                        Name = c.DisplayName,
                        UserId = c.DisplayName, // Hack this since we don't have other unique ID
                        MpnsChannel = String.Empty,                        
                        EmailAddresses = emailAddresses.ToString()
                    };
                })
                .ToList();

            // Databinding won't fire until the first time the user 
            // interacts with the list pickers, so trigger 
            // this manually
            if (Contacts != null && Contacts.Count != 0)
            {
                CurrentUser = Contacts.First();
            }
        }

        async void FriendsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentUserPropertyName)
            {
                // We haven't seen this user before
                if (CurrentUser.Id == 0)
                {
                    CurrentUser.MpnsChannel = PushChannel.ToString();
                    await chatService.CreateUserAsync(CurrentUser);
                }
                ReadFriends();
            }
            else if (e.PropertyName == PushChannelPropertyName)
            {
                RaisePropertyChanged(ContactsPropertyName);
                notificationService.RegisterNotificationHubs(PushChannel.ToString());
            }
        }

      

        private async void ReadFriends()
        {
            if (CurrentUser != null && CurrentUser.UserId != null)
            {
                Friends = await chatService.ReadFriendsAsync(CurrentUser.UserId);
            }
        }

        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Debug.WriteLine(e.ChannelUri.ToString());
            PushChannel = e.ChannelUri;
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Debug.WriteLine("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData);
        }
    }
}
