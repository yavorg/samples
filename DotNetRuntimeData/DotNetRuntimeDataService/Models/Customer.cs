using DotNetRuntimeDataService.DataObjects;
using System.Collections.Generic;

namespace DotNetRuntimeDataService.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        
        public string Name { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual MyEntityData EntityData { get; set; }
    }
}