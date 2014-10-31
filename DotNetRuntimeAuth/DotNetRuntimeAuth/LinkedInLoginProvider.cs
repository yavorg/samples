using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Newtonsoft.Json.Linq;
using Owin;
using Owin.Security.Providers.LinkedIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace DotNetRuntimeAuth
{
    public class LinkedInLoginProvider : LoginProvider
    {
        internal const string ProviderName = "LinkedIn";

        public LinkedInLoginProvider(IServiceTokenHandler tokenHandler)
            : base(tokenHandler)
        {
        }

        public override string Name
        {
            get { return ProviderName; }
        }

        public override void ConfigureMiddleware(IAppBuilder appBuilder,
            ServiceSettingsDictionary settings)
        {
            LinkedInAuthenticationOptions options = new LinkedInAuthenticationOptions
            {
                ClientId = settings["LinkedInClientId"],
                ClientSecret = settings["LinkedInClientSecret"],
                AuthenticationType = this.Name,
                Provider = new LinkedInLoginAuthenticationProvider()
            };
            appBuilder.UseLinkedInAuthentication(options);
        }

        public override ProviderCredentials CreateCredentials(
            ClaimsIdentity claimsIdentity)
        {
            Claim name = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Claim providerAccessToken = claimsIdentity
                .FindFirst(ServiceClaimTypes.ProviderAccessToken);

            LinkedInCredentials credentials = new LinkedInCredentials
            {
                UserId = this.TokenHandler.CreateUserId(this.Name, name != null ?
                    name.Value : null),
                AccessToken = providerAccessToken != null ?
                    providerAccessToken.Value : null
            };

            return credentials;
        }

        public override ProviderCredentials ParseCredentials(JObject serialized)
        {
            return serialized.ToObject<LinkedInCredentials>();
        }
    }
}