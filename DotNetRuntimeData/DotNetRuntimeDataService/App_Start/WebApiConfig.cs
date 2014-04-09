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
                cfg.CreateMap<MobileOrder, Order>()
                    .AfterMap((mobileOrder, order) =>
                    {
                        // In case of an insert
                        if (order.EntityData == null)
                        {
                            order.EntityData = new MyEntityData()
                            {
                                Id = mobileOrder.Id
                            };
                        }

                        order.EntityData.UpdatedAt = mobileOrder.UpdatedAt;
                    });

                cfg.CreateMap<MobileCustomer, Customer>()
                    .AfterMap((mobileCustomer, customer) =>
                    {
                        // In case of an insert
                        if (customer.EntityData == null)
                        {
                            customer.EntityData = new MyEntityData()
                            {
                                Id = mobileCustomer.Id
                            };
                        }

                        customer.EntityData.UpdatedAt = mobileCustomer.UpdatedAt;
                    });

                cfg.CreateMap<Order, MobileOrder>()
                    .ForMember(dst => dst.Id, map => map.MapFrom(x => x.EntityData.Id))
                    .ForMember(dst => dst.UpdatedAt, map => map.MapFrom(x => x.EntityData.UpdatedAt))
                    .ForMember(dst => dst.CreatedAt, map => map.MapFrom(x => x.EntityData.CreatedAt))
                    .ForMember(dst => dst.Version, map => map.MapFrom(x => x.EntityData.Version))
                    .ForMember(dst => dst.Deleted, map => map.MapFrom(x => x.EntityData.Deleted))
                    .ForMember(dst => dst.MobileCustomerId, map => map.MapFrom(x => x.Customer.EntityData.Id))
                    .ForMember(dst => dst.MobileCustomerName, map => map.MapFrom(x => x.Customer.Name));

                cfg.CreateMap<Customer, MobileCustomer>()
                    .ForMember(dst => dst.Id, map => map.MapFrom(x => x.EntityData.Id))
                    .ForMember(dst => dst.UpdatedAt, map => map.MapFrom(x => x.EntityData.UpdatedAt))
                    .ForMember(dst => dst.CreatedAt, map => map.MapFrom(x => x.EntityData.CreatedAt))
                    .ForMember(dst => dst.Version, map => map.MapFrom(x => x.EntityData.Version))
                    .ForMember(dst => dst.Deleted, map => map.MapFrom(x => x.EntityData.Deleted));


            });

            Database.SetInitializer(new DotNetRuntimeDataInitializer());
            Database.SetInitializer(new ExistingInitializer());

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


            List<MyEntityData> properties = new List<MyEntityData>
            {
                new MyEntityData { Order = orders[0], Id = Guid.NewGuid().ToString()},
                new MyEntityData { Order = orders[1], Id = Guid.NewGuid().ToString()},
                new MyEntityData { Order = orders[2], Id = Guid.NewGuid().ToString()},
                new MyEntityData { Customer = customers[0], Id = Guid.NewGuid().ToString()},
                new MyEntityData { Customer = customers[1], Id = Guid.NewGuid().ToString()},
                new MyEntityData { Customer = customers[2], Id = Guid.NewGuid().ToString()}
            };

            foreach (MyEntityData prop in properties)
            {
                context.EntityDatas.Add(prop);
            }

            base.Seed(context);
        }
    }
}

