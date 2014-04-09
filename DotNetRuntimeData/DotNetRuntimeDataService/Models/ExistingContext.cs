using DotNetRuntimeDataService.DataObjects;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace DotNetRuntimeDataService.Models
{
    public class ExistingContext : DbContext
    {
        public ExistingContext()
            : base("ExistingContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));

            modelBuilder.Entity<MyEntityData>()
                .HasOptional(e => e.Order)
                .WithOptionalDependent(o => o.EntityData);
            modelBuilder.Entity<MyEntityData>()
                .HasOptional(e => e.Customer)
                .WithOptionalDependent(o => o.EntityData);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<MyEntityData> EntityDatas { get; set; }
    }
}