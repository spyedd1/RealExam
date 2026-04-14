namespace GFLHApp.Models
{
    public class Orders
    {
        public int OrdersId { get; set; } // PK
        public string UserId { get; set; } // FK that links orders to the user account

        public DateOnly OrderDate { get; set; }

        public string? DeliveryMethod { get; set; } // This uses a ? as they can pick any type of delivery method, and it is not required to be filled in

        public bool Delivery { get; set; }

        public bool Collection { get; set; }

        public decimal OrdersTotal { get; set; }

        public string TrackingStatus { get; set; }

        public bool TermsAccepted { get; set; } // Whether the user has accepted the terms and conditions at checkout

        public DateOnly? DateOfCollection { get; set; } // This uses a ? as the date of collection is not required to be filled in as they may pick delivery instead of collection

        // Billing/Delivery Address Fields

        public string BillingLine1 { get; set; }

        public string? BillingLine2 { get; set; } // This uses a ? as the second line of the address is not required to be filled in

        public string BillingCity { get; set; }

        public string BillingPostcode { get; set; }

        public string? DeliveryLine1 { get; set; }

        public string? DeliveryLine2 { get; set; } 

        public string? DeliveryCity { get; set; }

        public string? DeliveryPostcode { get; set; }

        // Navigation properties

        public ICollection<OrderProducts>? OrderProducts { get; set; } // An order can have multiple order products
    }
}
