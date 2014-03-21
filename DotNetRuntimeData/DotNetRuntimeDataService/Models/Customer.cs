using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetRuntimeDataService.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        
        public string Name { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}