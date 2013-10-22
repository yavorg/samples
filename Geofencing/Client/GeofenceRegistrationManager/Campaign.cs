using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzure
{
    public class Campaign
    {
        public int Id {get; set;}

        [JsonProperty(PropertyName = "fenceName")]
        public string FenceName { get; set; }

        [JsonProperty(PropertyName = "url")]
        public Uri Url { get; set; }


    }
}
