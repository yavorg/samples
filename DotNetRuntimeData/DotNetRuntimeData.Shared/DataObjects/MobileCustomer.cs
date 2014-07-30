using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetRuntimeData
{
    public class MobileCustomer
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [CreatedAt]
        public DateTimeOffset? CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }
        
        [Version]
        public string Version { get; set; }

    }

}
