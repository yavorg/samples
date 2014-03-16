using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DotNetRuntimeDataService.Models
{
    public class ExistingContext : DbContext
    {
        public ExistingContext()
            : base("ExistingContext")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<SystemProperty> SystemProperties { get; set; }
    }
}