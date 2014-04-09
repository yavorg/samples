using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using DotNetRuntimeDataService.DataObjects;
using DotNetRuntimeDataService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service.Tables;

namespace DotNetRuntimeDataService.Controllers
{
    public class MobileCustomerController : TableController<MobileCustomer>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new ExistingContext();
            DomainManager = new MobileCustomerDomainManager(context, Request, Services);
        }

        public IQueryable<MobileCustomer> GetAllMobileCustomers()
        {
            return Query();
        }

        public SingleResult<MobileCustomer> GetMobileCustomer(string id)
        {
            return Lookup(id);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        protected override Task<MobileCustomer> InsertAsync(MobileCustomer item)
        {
            return base.InsertAsync(item);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        protected override Task<MobileCustomer> UpdateAsync(string id, Delta<MobileCustomer> patch)
        {
            return base.UpdateAsync(id, patch);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        protected override Task DeleteAsync(string id)
        {
            return base.DeleteAsync(id);
        }
    }
}