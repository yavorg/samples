using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace SlapChat.Model
{
    [DataTable("photoRecords")]
    public class PhotoRecord
    {
        public int Id
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "senderUserId")]
        public string SenderUserId
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

        [JsonProperty(PropertyName = "recepientUserId")]
        public string RecepientUserId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "photoContentSecretId")]
        public string PhotoContentSecretId
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

        [JsonProperty(PropertyName = "uri")]
        public Uri Uri
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "uploadKey")]
        public string UploadKey
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
