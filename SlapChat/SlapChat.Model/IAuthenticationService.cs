using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlapChat.Model
{
    public interface IAuthenticationService
    {

        Task<MobileServiceUser> LoginAsync(string token);

        Task<MobileServiceUser> LoginEasyAsync();
    }
}
