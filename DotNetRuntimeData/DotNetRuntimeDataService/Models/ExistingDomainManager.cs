﻿using DotNetRuntimeDataService.DataObjects;
using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using System.Data.Entity;

namespace DotNetRuntimeDataService.Models
{
    public class ExistingDomainManager : MappedEntityDomainManager<MobileOrder, Order>
    {
        private ExistingContext context;

        public ExistingDomainManager(ExistingContext context, HttpRequestMessage request, ApiServices services)
            : base(context, request, services)
        {
            Request = request;
            this.context = context;
        }

        public override SingleResult<MobileOrder> Lookup(string id)
        {
            int orderId = GetKey<int>(id);
            return LookupEntity(o => o.OrderId == orderId);
        }

        public override async Task<MobileOrder> InsertAsync(MobileOrder data)
        {
            Customer[] customers = await this.context.Customers.Where(c => c.Name.Equals(data.CustomerName, StringComparison.Ordinal)).ToArrayAsync();
            if (customers.Length == 0)
            {
                throw new HttpResponseException(Request.CreateBadRequestResponse("Customer with name '{0}' was not found", data.CustomerName));
            }

            return await base.InsertAsync(data);
        }

        public override Task<MobileOrder> UpdateAsync(string id, Delta<MobileOrder> patch)
        {
            int orderId = GetKey<int>(id);
            return UpdateEntityAsync(patch, orderId);
        }

        public override async Task<MobileOrder> ReplaceAsync(string id, MobileOrder data)
        {
            Customer[] customers = await this.context.Customers.Where(c => string.Equals(c.Name, data.CustomerName, StringComparison.Ordinal)).ToArrayAsync();
            if (customers.Length == 0)
            {
                throw new HttpResponseException(Request.CreateBadRequestResponse("Customer with name '{0}' was not found", data.CustomerName));
            }

            return await base.ReplaceAsync(id, data);
        }

        public override Task<bool> DeleteAsync(string id)
        {
            int orderId = GetKey<int>(id);
            return DeleteItemAsync(orderId);
        }
    }
}