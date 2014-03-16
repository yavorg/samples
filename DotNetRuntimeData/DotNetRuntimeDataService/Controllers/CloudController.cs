using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetRuntimeDataService.Controllers
{
    public class CloudController : ApiController
    {
        public ApiServices Services { get; set; }

        [RequiresAuthorization(AuthorizationLevel.User)]
        public string Get()
        {

            return "Hello";
        }
    }
}
