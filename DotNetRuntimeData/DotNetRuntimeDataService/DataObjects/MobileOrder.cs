using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MobileOrder : EntityData
    {
        public string Item { get; set; }

        public int Quantity { get; set; }

        public bool Completed { get; set; }

        [JsonIgnore]
        public int CustomerId { get; set; }

        [Required]
        public string MobileCustomerId { get; set; }

        public string MobileCustomerName { get; set; }
    }
}