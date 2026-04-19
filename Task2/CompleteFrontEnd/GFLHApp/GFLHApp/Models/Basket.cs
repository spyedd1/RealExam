namespace GFLHApp.Models
{
    public class Basket
    {

        public int BasketId { get; set; } // PK

        public string UserId { get; set; } // FK that links the basket to the user account

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties

        public ICollection<BasketProducts>? BasketProducts { get; set; } // A basket can have multiple basket products

    }
}
