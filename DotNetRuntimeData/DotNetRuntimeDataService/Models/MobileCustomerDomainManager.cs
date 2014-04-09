using DotNetRuntimeDataService.DataObjects;
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
using AutoMapper;

namespace DotNetRuntimeDataService.Models
{
    public class MobileCustomerDomainManager : MappedEntityDomainManager<MobileCustomer, Customer>
    {
        private ExistingContext context;

        public MobileCustomerDomainManager(ExistingContext context, HttpRequestMessage request, ApiServices services)
            : base(context, request, services)
        {
            Request = request;
            this.context = context;
        }

        public static int GetKey(string mobileCustomerId, DbSet<MyEntityData> entityDatas, HttpRequestMessage request)
        {
            int customerId = entityDatas
               .Where(c => c.Id == mobileCustomerId)
               .Select(c => c.Customer.CustomerId)
               .FirstOrDefault();

            if (customerId == 0)
            {
                throw new HttpResponseException(request.CreateNotFoundResponse());
            }
            return customerId;
        }

        protected override T GetKey<T>(string mobileCustomerId)
        {
            return (T)(object)GetKey(mobileCustomerId, this.context.EntityDatas, this.Request);
        }

        public override SingleResult<MobileCustomer> Lookup(string mobileCustomerId)
        {
            int customerId = GetKey<int>(mobileCustomerId);
            return LookupEntity(c => c.CustomerId == customerId);
        }

        public override async Task<MobileCustomer> InsertAsync(MobileCustomer mobileCustomer)
        {
            return await base.InsertAsync(mobileCustomer);
        }

        public override async Task<MobileCustomer> UpdateAsync(string mobileCustomerId, Delta<MobileCustomer> patch)
        {
            int customerId = GetKey<int>(mobileCustomerId);

            Customer existingCustomer = await this.Context.Set<Customer>().FindAsync(customerId);
            if (existingCustomer == null)
            {
                throw new HttpResponseException(this.Request.CreateNotFoundResponse());
            }

            MobileCustomer existingCustomerDTO = Mapper.Map<Customer, MobileCustomer>(existingCustomer);
            patch.Patch(existingCustomerDTO);
            Mapper.Map<MobileCustomer, Customer>(existingCustomerDTO, existingCustomer);

            // this is required to force version update
            existingCustomer.EntityData.UpdatedAt = DateTimeOffset.UtcNow;

            await this.SubmitChangesAsync();

            // this is required to send the new version to client
            MobileCustomer updatedCustomerDTO = Mapper.Map<Customer, MobileCustomer>(existingCustomer);

            return updatedCustomerDTO;
        }

        public override async Task<MobileCustomer> ReplaceAsync(string mobileCustomerId, MobileCustomer mobileCustomer)
        {
            return await base.ReplaceAsync(mobileCustomerId, mobileCustomer);
        }

        public override async Task<bool> DeleteAsync(string mobileCustomerId)
        {
            int customerId = GetKey<int>(mobileCustomerId);
            MyEntityData data = this.context.EntityDatas.FirstOrDefault(x => x.Id == mobileCustomerId);
            if (data != null)
            {
                // need to delete this first in order for delete to work
                this.context.EntityDatas.Remove(data);
            }

            return await DeleteItemAsync(customerId);
        }
    }
}