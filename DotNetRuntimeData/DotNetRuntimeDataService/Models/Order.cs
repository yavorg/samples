using DotNetRuntimeDataService.DataObjects;

namespace DotNetRuntimeDataService.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Item { get; set; }

        public int Quantity { get; set; }

        public int CustomerId { get; set; }
      
        public virtual Customer Customer { get; set; }

        public virtual MyEntityData EntityData { get; set; }

    }
}