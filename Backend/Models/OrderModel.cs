using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace RestaurantApp.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; } = null!;
        [Required]
        [Range(0.01, 100000.00)]
        public decimal TotalAmount { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Preparing, Out for Delivery, Delivered, Cancelled
        [MaxLength(50)]
        [Required]
        public string? PaymentMethod { get; set; } = string.Empty; // e.g., Credit Card, PayPal, Cash on Delivery
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Refunded
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; } = null!;
        public List<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
       
        public DateTime? UpdatedAt { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public decimal GST { get; set; } // Goods and Services Tax
        public decimal FinalBillAmount { get; set; } // TotalAmount + GST - Discounts
        public string DeliveryStatus { get; set; } = "Pending"; // Pending, Confirmed, Preparing, Out for Delivery, Delivered, Cancelled
        public decimal DeliveryFee { get; set; } = 0.00m; // Delivery Fee   
        public string CouponCode { get; set; } = string.Empty; // Coupon Code   
    }
}