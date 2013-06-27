using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace SlapChat.Model
{
    [DataTable("photoContents")]
    public class PhotoContent
    {
        public int Id
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "secretId")]
        public string SecretId
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

        [JsonProperty(PropertyName = "photoRecordId")]
        public int PhotoRecordId
        {
            get;
            set;
        }

    }
}
