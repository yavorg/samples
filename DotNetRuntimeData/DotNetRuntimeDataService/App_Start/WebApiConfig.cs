using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using DotNetRuntimeDataService.DataObjects;
using DotNetRuntimeDataService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using AutoMapper;
using DotNetRuntimeDataService.DataObjects;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System.Data.SqlClient;

namespace DotNetRuntimeDataService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during developemnt, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            Mapper.Initialize(cfg =>
            {

                // TODO try remove this
                //cfg.CreateMap<SystemProperty, MobileOrder>()
                //   .ForMember(dst => dst.CreatedAt, map => map.MapFrom(src => src.CreatedAt));
                //cfg.CreateMap<SystemProperty, MobileOrder>()
                //    .ForMember(dst => dst.Deleted, map => map.MapFrom(src => src.Deleted));
                //cfg.CreateMap<SystemProperty, MobileOrder>()
                //    .ForMember(dst => dst.UpdatedAt, map => map.MapFrom(src => src.UpdatedAt));
                //cfg.CreateMap<SystemProperty, MobileOrder>()
                //    .ForMember(dst => dst.Version, map => map.MapFrom(src => src.Version));
                //cfg.CreateMap<SystemProperty, MobileOrder>()
                //.ForMember(dst => dst.Id, map => map.MapFrom(src => src.Id));

                cfg.CreateMap<MobileOrder, SystemProperty>()
                    .ForMember(dst => dst.CreatedAt, map => map.MapFrom(src => src.CreatedAt));
                cfg.CreateMap<MobileOrder, SystemProperty>()
                    .ForMember(dst => dst.Deleted, map => map.MapFrom(src => src.Deleted));
                cfg.CreateMap<MobileOrder, SystemProperty>()
                    .ForMember(dst => dst.UpdatedAt, map => map.MapFrom(src => src.UpdatedAt));
                cfg.CreateMap<MobileOrder, SystemProperty>()
                    .ForMember(dst => dst.Version, map => map.MapFrom(src => src.Version));
                cfg.CreateMap<MobileOrder, SystemProperty>()
                   .ForMember(dst => dst.Id, map => map.MapFrom(src => src.Id));

                cfg.CreateMap<MobileOrder, Order>()
                    .ForMember(dst => dst.Property, map => map.MapFrom(x => Mapper.Map<MobileOrder, SystemProperty>(x)));

                cfg.CreateMap<Order, MobileOrder>()
                    .ForMember(dst => dst.Id, map => map.MapFrom(x => x.Property.Id))
                    .ForMember(dst => dst.UpdatedAt, map => map.MapFrom(x => x.Property.UpdatedAt))
                    .ForMember(dst => dst.CreatedAt, map => map.MapFrom(x => x.Property.CreatedAt))
                    .ForMember(dst => dst.Version, map => map.MapFrom(x => x.Property.Version))
                    .ForMember(dst => dst.Deleted, map => map.MapFrom(x => x.Property.Deleted));
 
            });

            Database.SetInitializer(new DotNetRuntimeDataInitializer());
            Database.SetInitializer(new ExistingInitializer());

            /*
            string SqlClientProvider = "System.Data.SqlClient";
            DbConfiguration.Loaded += (sender, e) => e.ReplaceService<Func<MigrationSqlGenerator>>(
                (s, k) => SqlClientProvider.Equals(k as string, StringComparison.OrdinalIgnoreCase) && s().GetType() == typeof(SqlServerMigrationSqlGenerator) ? () => new EntityTableSqlGenerator() : s);
            */
            
        }

    }

    public class DotNetRuntimeDataInitializer : DropCreateDatabaseIfModelChanges<DotNetRuntimeDataContext>
    {
        protected override void Seed(DotNetRuntimeDataContext context)
        {
            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = "1", Text = "First item", Complete = false },
                new TodoItem { Id = "2", Text = "Second item", Complete = false },
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            base.Seed(context);
        }
    }

    public class ExistingInitializer : DropCreateDatabaseIfModelChanges<ExistingContext>
    {
           
        
        protected override void Seed(ExistingContext context)
        {
            List<Order> orders = new List<Order>
            {
                new Order { OrderId = 10, Item = "Shoes", Quantity = 2 },
                new Order { OrderId = 20, Item = "Polos", Quantity = 10 },
                new Order { OrderId = 30, Item = "S'mores", Quantity = 20 }
            };

            List<Customer> customers = new List<Customer>
            {
                new Customer { CustomerId = 1, Name = "Henrik", Orders = new Collection<Order> { 
                    orders[0]}},
                new Customer { CustomerId = 2, Name = "Scott", Orders = new Collection<Order> { 
                    orders[1]}},
                new Customer { CustomerId = 3, Name = "Benjamin", Orders = new Collection<Order> { 
                    orders[2]}},
            };

            List<SystemProperty> properties = new List<SystemProperty>
            {
                new SystemProperty { Order = orders[0], Id = "one"},
                new SystemProperty { Order = orders[1], Id = "two"},
                new SystemProperty { Order = orders[2], Id = "three"}
            };

            foreach (Customer customer in customers)
            {
                context.Customers.Add(customer);
            }

            foreach (SystemProperty prop in properties)
            {
                context.SystemProperties.Add(prop);
            }

            base.Seed(context);
        }
    }
}

