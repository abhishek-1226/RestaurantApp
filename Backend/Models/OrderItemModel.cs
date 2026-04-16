namespace RestaurantApp.Models;

    public class OrderItemModel
    {
        public int Id { get; set; } // Primary key
        public int MenuItemId { get; set; } // Foreign key to the MenuItemModel
        public string MenuItemName { get; set; } // Name of the menu item at the time of order
        public int Quantity { get; set; } // Quantity of the menu item ordered
        public MenuItemModel MenuItem { get; set; } // Navigation property to the MenuItemModel
        public int OrderId { get; set; } // Foreign key to the OrderModel
        public OrderModel Order { get; set; } // Navigation property to the OrderModel
        public string? SpecialInstructions { get; set; } // Any special instructions for this order item
        public decimal UnitPrice { get; set; } // Price of a single unit at the time of order
    }
