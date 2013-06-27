using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    public interface INotificationService
    {
        void RegisterNotificationHubs(string channel);
    }
}
