using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    class AuthenticationService : IAuthenticationService
    {
        private MobileServiceClient client;

        public AuthenticationService()
        {
            client = new MobileServiceClient(MobileServiceConfig.ApplicationUri,
             MobileServiceConfig.ApplicationKey);
        }

        public async Task<MobileServiceUser> LoginAsync(string token)
        {
            return await client.LoginWithMicrosoftAccountAsync(token);
        }


        public async Task<MobileServiceUser> LoginEasyAsync()
        {
            return await client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
        }
    }
}
