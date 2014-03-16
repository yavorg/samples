using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MobileOrder : EntityData
    {
        public string Item { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string CustomerName { get; set; }
    }
}