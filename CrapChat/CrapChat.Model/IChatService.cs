using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CrapChat.Model
{
    public interface IChatService
    {
        ObservableCollection<Friend> LoadFriends();
        void AddFriends(IEnumerable<Friend> newFriends);
    }
}
