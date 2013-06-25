using CrapChat.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.UserData;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrapChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        private IChatService chatService;

        public FriendsViewModel()
        {
            chatService = ServiceLocator.Current.GetInstance<IChatService>();

            InviteContacts = new RelayCommand(() =>
            {
                Contacts contacts = new Contacts();
                contacts.SearchCompleted += contacts_SearchCompleted;
                contacts.SearchAsync(String.Empty, FilterKind.None, null);
            });

            RefreshCommand = new RelayCommand(() =>
            {
                RaisePropertyChanged(FriendsPropertyName);
            });
            
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
        public ObservableCollection<Friend> Friends
        {
            get
            {
                return chatService.ReadFriends();
            }
        }


        void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            Dictionary<Contact, string> contactsMicrosoftAccount = new Dictionary<Contact, string>();
            List<Friend> matches = e.Results
                .Where((c) => c.EmailAddresses.Any((a) =>
                {
                    if (a.EmailAddress.EndsWith("live.com") || a.EmailAddress.EndsWith("outlook.com"))
                    {
                        // If they have multiple Microsoft Accounts, we will just pick the 
                        // one that comes up last
                        contactsMicrosoftAccount[c] = a.EmailAddress;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }))
                .Select<Contact, Friend>((c) =>
                {
                    return new Friend
                    {
                        Name = c.DisplayName,
                        MicrosoftAccount = contactsMicrosoftAccount[c]
                    };
                })
                .ToList();

            chatService.CreateFriends(matches);
            
            // Trigger reload of friends
            RaisePropertyChanged(FriendsPropertyName);
            
        }

    }
}
