using AutoMapper;
using DotNetRuntimeDataService.DataObjects;
using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

namespace DotNetRuntimeDataService.Models
{
    public class MobileOrderDomainManager : MappedEntityDomainManager<MobileOrder, Order>
    {
        private ExistingContext context;

        public MobileOrderDomainManager(ExistingContext context, HttpRequestMessage request, ApiServices services)
            : base(context, request, services)
        {
            Request = request;
            this.context = context;
        }
 
        protected override T GetKey<T>(string mobileOrderId)
        {
            int orderId = this.context.EntityDatas.Where(d => d.Id == mobileOrderId).Select(d => d.Order.OrderId).FirstOrDefault();
            if (orderId == 0)
            {
                throw new HttpResponseException(this.Request.CreateNotFoundResponse());
            }
            return (T)(object)orderId;
        }

        public override SingleResult<MobileOrder> Lookup(string mobileOrderId)
        {
            int orderId = GetKey<int>(mobileOrderId);
            return LookupEntity(o => o.OrderId == orderId);
        }

        private async Task<Customer> VerifyMobileCustomer(string mobileCustomerId, string mobileCustomerName)
        {
            int customerId = MobileCustomerDomainManager.GetKey(mobileCustomerId, this.context.EntityDatas, this.Request);
            Customer customer = await this.context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                throw new HttpResponseException(Request.CreateBadRequestResponse("Customer with name '{0}' was not found", mobileCustomerName));
            }
            return customer;
        }

        public override async Task<MobileOrder> InsertAsync(MobileOrder mobileOrder)
        {
            Customer customer = await VerifyMobileCustomer(mobileOrder.MobileCustomerId, mobileOrder.MobileCustomerName);
            mobileOrder.CustomerId = customer.CustomerId;
            return await base.InsertAsync(mobileOrder);
        }

        public override async Task<MobileOrder> UpdateAsync(string mobileOrderId, Delta<MobileOrder> patch)
        {
            Customer customer = await VerifyMobileCustomer(patch.GetEntity().MobileCustomerId, patch.GetEntity().MobileCustomerName);

            int orderId = GetKey<int>(mobileOrderId);

            Order existingOrder = await this.Context.Set<Order>().FindAsync(orderId);
            if (existingOrder == null)
            {
                throw new HttpResponseException(this.Request.CreateNotFoundResponse());
            }

            MobileOrder existingOrderDTO = Mapper.Map<Order, MobileOrder>(existingOrder);
            patch.Patch(existingOrderDTO);
            Mapper.Map<MobileOrder, Order>(existingOrderDTO, existingOrder);

            // This is required to force version update
            existingOrder.EntityData.UpdatedAt = DateTimeOffset.UtcNow;

            // This is required to map the right Id for the customer
            existingOrder.CustomerId = customer.CustomerId;

            await this.SubmitChangesAsync();

            // this is required to send the new version to client
            MobileOrder updatedOrderDTO = Mapper.Map<Order, MobileOrder>(existingOrder);

            return updatedOrderDTO;
        }

        public override async Task<MobileOrder> ReplaceAsync(string mobileOrderId, MobileOrder mobileOrder)
        {
            await VerifyMobileCustomer(mobileOrder.MobileCustomerId, mobileOrder.MobileCustomerName);

            return await base.ReplaceAsync(mobileOrderId, mobileOrder);
        }

        public override Task<bool> DeleteAsync(string mobileOrderId)
        {
            int orderId = GetKey<int>(mobileOrderId);
            return DeleteItemAsync(orderId);
        }
    }
}