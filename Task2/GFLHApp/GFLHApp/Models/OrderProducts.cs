namespace GFLHApp.Models
{
    public class OrderProducts
    {
        public int OrderProductsId { get; set; } // PK
        public int OrdersId { get; set; } // FK that links the order products to the orders
        public int ProductsId { get; set; } // FK that links the order products to the product
        public int ProductQuantity { get; set; }

        // Navigation properties
        public Products Products { get; set; } // An order product is associated with one product
        public Orders Orders { get; set; } // An order product is associated with one order
    }
}
