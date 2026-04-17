namespace GFLHApp.Models
{
    public class Producers
    {
        public int ProducersId { get; set; } // PK

        public string UserId { get; set; } // FK that links producers to the user account

        public string ProducerName { get; set; } // The name of the producer, which may be a business name or an individual's name

        public string ProducerEmail { get; set; } // The email address of the producer, used for communication and notifications

        public string ProducerInformation { get; set; } // Additional information about the producer, such as a description of their products, farming practices, or any certifications they may have

        public string? VATNumber { get; set; } // VAT registration number for the producer, if applicable
        public bool IsVATRegistered { get; set; } // Whether the producer is VAT registered

        // Navigation properties

        public ICollection<Products>? Products { get; set; } // A producer can have multiple products

        public ICollection<ProducerOrders> ProducerOrders { get; set; } // A producer can have multiple producer orders
    }
}
