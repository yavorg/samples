using CrapChat.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrapChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        public FriendsViewModel()
        {
            InviteContacts = new RelayCommand(() =>
                {
                    Contacts contacts = new Contacts();
                    contacts.SearchCompleted += contacts_SearchCompleted;
                    contacts.SearchAsync(String.Empty, FilterKind.None, null);
                });

            MessengerInstance.Register<FriendsMessage>(this, (m) =>
            {
                Friends.Clear();
                foreach(Friend f in m.Friends){
                    Friends.Add(f);
                }
            });

            Friends = new ObservableCollection<Friend>();

        }

        public RelayCommand InviteContacts
        {
            get;
            private set;
        }

      
        public const string FriendsPropertyName = "Friends";
        private ObservableCollection<Friend> friends;

        public ObservableCollection<Friend> Friends
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
            List<string> matches = e.Results
                .SelectMany<Contact, string>((c) => {
                    return c.EmailAddresses
                        .Where((a) => {
                            return a.EmailAddress.EndsWith("live.com") || a.EmailAddress.EndsWith("outlook.com");
                        })
                        .Select<ContactEmailAddress, string>((a) => a.EmailAddress);
                }).ToList();

             List<Friend> friendsToAdd = matches.Select<string, Friend>((s) =>
                {
                    return new Friend()
                    {
                        Name = "Dummy Friend",
                        MicrosoftAccount = s
                    };
                }).ToList();

             friendsToAdd.ForEach((c) =>
                 {
                     Friends.Add(c);
                 });

            // Broadcast updated friend list for other ViewModels
            MessengerInstance.Send<FriendsMessage>(
                new FriendsMessage{ Friends = new List<Friend>(Friends) }
            );

            
        }

    }
}
