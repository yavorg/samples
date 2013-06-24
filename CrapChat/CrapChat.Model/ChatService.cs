using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrapChat.Model
{
    public class ChatService : IChatService
    {
        private List<Friend> friends;

        public ChatService()
	    {
            friends = new List<Friend>();
	    }

        public ObservableCollection<Friend> LoadFriends()
        {
            return new ObservableCollection<Friend>(friends);
        }

        public void AddFriends(IEnumerable<Friend> newFriends)
        {
            friends.AddRange(newFriends);
        }
    }
}
