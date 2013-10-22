using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzure
{
    class EnterRequest
    {
        [JsonProperty(PropertyName = "fenceName")]
        public string FenceName { get; set; }

    }
}
