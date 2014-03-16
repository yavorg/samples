using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using DotNetRuntimeDataService.DataObjects;
using DotNetRuntimeDataService.Models;

namespace DotNetRuntimeDataService.Controllers
{
    public class MongoTodoItemController : TableController<MongoTodoItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            string tableName = controllerContext.ControllerDescriptor.ControllerName;
            DomainManager = new MongoDomainManager<MongoTodoItem>("Mongo", "HenrikNMongoLab", "MongoTodoItem", Request, Services);
        }

        // GET tables/MongoTodoItem
        public IQueryable<MongoTodoItem> GetAllMongoTodoItems()
        {
            return Query();
        }

        // GET tables/MongoTodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<MongoTodoItem> GetMongoTodoItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/MongoTodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<MongoTodoItem> PatchMongoTodoItem(string id, Delta<MongoTodoItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/MongoTodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostMongoTodoItem(MongoTodoItem item)
        {
            MongoTodoItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/MongoTodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMongoTodoItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}