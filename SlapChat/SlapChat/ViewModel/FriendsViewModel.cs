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

namespace SlapChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        private IChatService chatService;

        public FriendsViewModel()
        {
            chatService = ServiceLocator.Current.GetInstance<IChatService>();

            Contacts contacts = new Contacts();
            contacts.SearchCompleted += contacts_SearchCompleted;
            contacts.SearchAsync(String.Empty, FilterKind.None, null);

            InviteContacts = new RelayCommand(() =>
            {
               
            });

            RefreshCommand = new RelayCommand(() =>
            {
                RaisePropertyChanged(ContactsPropertyName);
                RaisePropertyChanged(FriendsPropertyName);
            });

            PropertyChanged += FriendsViewModel_PropertyChanged;


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


      
        public const string FriendsPropertyName = "Friends";
        public ObservableCollection<User> Friends
        {
            get
            {
                if (CurrentUser != null)
                {
                    return chatService.ReadFriends(CurrentUser.UserId);
                }
                else
                {
                    return null;
                }
            }
        }

        public const string HaveFriendsPropertyName = "HaveFriends";
        public bool HaveFriends
        {
            get
            {
                return Friends != null && Friends.Count != 0;
            }
        }


        void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            Contacts = e.Results.Select<Contact, User>((c) =>
                {
                    StringBuilder emailAddresses = new StringBuilder();
                    foreach(ContactEmailAddress a in c.EmailAddresses){
                        emailAddresses.Append(a.EmailAddress).Append(" ");
                    }

                    return new User
                    {
                        Name = c.DisplayName,
                        UserId = c.DisplayName, // Hack this since we don't have other unique ID
                        EmailAddresses = emailAddresses.ToString()
                    };
                })
                .ToList();   
        }




        void FriendsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentUserPropertyName)
            {
                chatService.CreateUser(CurrentUser);

                // Trigger reload of friends
                RaisePropertyChanged(FriendsPropertyName);
                RaisePropertyChanged(HaveFriendsPropertyName);

                
            }
        }

    }
}
