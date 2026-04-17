namespace GFLHApp.Models
{
    public class ProducerOrders
    {
        public int ProducerOrdersId { get; set; } // Primary key
        public int OrdersId { get; set; }          // FK links back to the parent order
        public string ProducerId { get; set; }     // FK links to the producer (UserId from Producers)
        public decimal ProducerSubtotal { get; set; }  // their slice of the total

        public string TrackingStatus { get; set; } = "Pending"; // "Pending", "Accepted", "Cancelled"

        // Navigation links
        public Orders Orders { get; set; } // Navigation property to the parent order, linked by OrdersId
        public Producers Producers { get; set; } // Navigation property to the producer, linked by ProducerId (UserId)

        public ICollection<OrderProducts> OrderProducts { get; set; } // Navigation property back to order products
    }
}
