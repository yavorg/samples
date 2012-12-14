using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace SeattleSkiBuddy
{
    [DataTable(Name="updates")]
    class Update
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "twitterId")]
        public long TwitterId { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "date")]
        public DateTimeOffset Date { get; set; }
    }
}
