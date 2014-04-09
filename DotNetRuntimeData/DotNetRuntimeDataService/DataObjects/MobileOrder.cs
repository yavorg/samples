using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MobileOrder : EntityData
    {
        public string Item { get; set; }

        public int Quantity { get; set; }

        [JsonIgnore]
        public int CustomerId { get; set; }

        [Required]
        public string MobileCustomerId { get; set; }

        public string MobileCustomerName { get; set; }
    }
}