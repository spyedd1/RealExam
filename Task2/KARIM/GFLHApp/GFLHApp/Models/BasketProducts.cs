namespace GFLHApp.Models
{
    public class BasketProducts
    {

        public int BasketProductsId { get; set; } // PK

        public int BasketId { get; set; } // FK that links the basket products to the basket

        public int ProductsId { get; set; } // FK that links the basket products to the product

        public int ProductQuantity { get; set; } 

        // Navigation properties

        public Products Products { get; set; } // A basket product is associated with one product

        public Basket Basket { get; set; } // A basket product is associated with one basket
    }
}
