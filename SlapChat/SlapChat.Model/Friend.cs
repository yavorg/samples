using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlapChat.Model
{
    [DataTable("friends")]
    public class Friend
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

        [JsonProperty(PropertyName = "microsoftaccount")]
        public string MicrosoftAccount
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
