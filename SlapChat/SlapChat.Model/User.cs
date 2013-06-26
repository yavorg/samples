using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlapChat.Model
{
    [DataTable("users")]
    public class User
    {
        public int Id
        {
            get;
            set;
        }

        [JsonProperty(PropertyName="name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "userId")]
        public string UserId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "emailAddresses")]
        public string EmailAddresses
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "mpnschannel")]
        public string MpnsChannel
        {
            get;
            set;
        }



    }
}
