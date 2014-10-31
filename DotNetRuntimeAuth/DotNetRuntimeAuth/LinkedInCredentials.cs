using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetRuntimeAuth
{
    public class LinkedInCredentials : ProviderCredentials
    {
        public LinkedInCredentials()
            : base(LinkedInLoginProvider.ProviderName)
        {
        }

        public string AccessToken { get; set; }
    }
}