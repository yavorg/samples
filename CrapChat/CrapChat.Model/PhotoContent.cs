using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace CrapChat.Model
{
    [DataTable("photoContents")]
    public class PhotoContent
    {
        public Guid Id
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

        [JsonProperty(PropertyName = "uploaded")]
        public bool Uploaded
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
