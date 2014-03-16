using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetRuntimeDataService.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Item { get; set; }

        public int Quantity { get; set; }

        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SystemProperty Property { get; set; }

    }
}