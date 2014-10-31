using Microsoft.WindowsAzure.Mobile.Service.Security;
using Owin.Security.Providers.LinkedIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace DotNetRuntimeAuth
{
    public class LinkedInLoginAuthenticationProvider : LinkedInAuthenticationProvider
    {
        public override Task Authenticated(LinkedInAuthenticatedContext context)
        {
            context.Identity.AddClaim(
                new Claim(ServiceClaimTypes.ProviderAccessToken, context.AccessToken));
            return base.Authenticated(context);
        }
    }
}