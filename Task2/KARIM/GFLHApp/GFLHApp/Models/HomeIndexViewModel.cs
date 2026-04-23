namespace GFLHApp.Models
{
    public class HomeIndexViewModel
    {
        public int GrowerCount { get; set; } // Number of growers in the system, derived from the Producers table.

        public int AvailableProductCount { get; set; } // Number of products marked as available, derived from the Products table.

        public int OrderCount { get; set; } // Total number of orders placed, derived from the Orders table.

        public int CategoryCount { get; set; } // Number of distinct non-empty product categories, derived from the Products table.
    }
}
