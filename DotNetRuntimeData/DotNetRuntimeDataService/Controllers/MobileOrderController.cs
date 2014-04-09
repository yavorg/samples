using DotNetRuntimeDataService.DataObjects;
using DotNetRuntimeDataService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace DotNetRuntimeDataService.Controllers
{
    public class MobileOrderController : TableController<MobileOrder>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            ExistingContext context = new ExistingContext();
            DomainManager = new MobileOrderDomainManager(context, Request, Services);
        }

        // GET tables/MobileOrder
        public IQueryable<MobileOrder> GetAllMobileOrders()
        {
            return Query();
        }

        // GET tables/MobileOrder/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<MobileOrder> GetMobileOrder(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/MobileOrder/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<MobileOrder> PatchMobileOrder(string id, Delta<MobileOrder> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/MobileOrder/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [ResponseType(typeof(MobileOrder))]
        public async Task<IHttpActionResult> PostMobileOrder(MobileOrder item)
        {
            MobileOrder current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/MobileOrder/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMobileOrder(string id)
        {
            return DeleteAsync(id);
        }
    }
}