using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetRuntimeData.DataObjects
{
    public class MobileOrder
    {
        public string Id { get; set; }

        public string Item { get; set; }

        public int Quantity { get; set; }

        public bool Completed { get; set; }

        public string MobileCustomerId { get; set; }

        public string MobileCustomerName { get; set; }

        [CreatedAt]
        public DateTimeOffset? CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }

        [Version]
        public string Version { get; set; }

    }
}
