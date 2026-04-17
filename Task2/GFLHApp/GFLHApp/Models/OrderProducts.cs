namespace GFLHApp.Models
{
    public class OrderProducts
    {
        public int OrderProductsId { get; set; } // PK
        public int OrdersId { get; set; } // FK that links the order products to the orders
        public int ProductsId { get; set; } // FK that links the order products to the product
        public int ProductQuantity { get; set; } // The quantity of product in this order
        public string? InvoiceNumber { get; set; } // Auto-generated invoice number for VAT registered producers

        public int? ProducerOrdersId { get; set; } // FK to the producer order slice, nullable for existing records



        // Navigation properties
        public Products Products { get; set; } // An order product is associated with one product
        public Orders Orders { get; set; } // An order product is associated with one order
        public ProducerOrders ProducerOrders { get; set; } // Navigation property to the producer order slice

    }
}
