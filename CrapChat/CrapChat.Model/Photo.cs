using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace CrapChat.Model
{
    [DataTable("photos")]
    public class Photo
    {
        public int Id
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "senderMicrosoftAccount")]
        public string SenderMicrosoftAccount
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "senderName")]
        public string SenderName
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "recepientMicrosoftAccount")]
        public string RecepientMicrosoftAccount
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "recepientName")]
        public string RecepientName
        {
            get;
            set;
        }


        [JsonProperty(PropertyName = "photo")]
        public Uri Uri
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "sent")]
        public DateTimeOffset Sent
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "received")]
        public DateTimeOffset Received
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "expired")]
        public bool Expired
        {
            get;
            set;
        }

    }
}
