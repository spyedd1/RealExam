namespace GFLHApp.Models
{
    public class Producers
    {
        public int ProducersId { get; set; } // PK

        public string UserId { get; set; } // FK that links producers to the user account

        public string ProducerName { get; set; }

        public string ProducerEmail { get; set; }

        public string ProducerInformation { get; set; }

        // Navigation properties

        public ICollection<Products>? Products { get; set; } // A producer can have multiple products
    }
}
