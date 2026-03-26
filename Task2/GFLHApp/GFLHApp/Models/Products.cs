namespace GFLHApp.Models
{
    public class Products
    {
        public int ProductsId { get; set; }

        public int ProducersId { get; set; } // FK that links products to the producer who created them

        public string ItemName { get; set; }

        public decimal ItemPrice { get; set; }

        public string? ImagePath { get; set; } // This uses a ? as the image path is not required to be filled in as they may not want to or be able to upload an image of their product

        public int QuantityInStock { get; set; }

        public bool Available { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        // Navigation properties

        public Producers Producers { get; set; } // A product is associated with one producer

        public ICollection<BasketProducts>? BasketProducts { get; set; } // A product can be in multiple basket products

        public ICollection<OrderProducts>? OrderProducts { get; set; } // A product can be in multiple order products
    }
}
