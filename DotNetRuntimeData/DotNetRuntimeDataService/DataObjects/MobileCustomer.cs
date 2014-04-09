using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MobileCustomer : EntityData
    {
        public string Name { get; set; }
    }
}