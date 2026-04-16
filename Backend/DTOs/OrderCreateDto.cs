using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantApp.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public int RestaurantId { get; set; }
        [Required]
        [MaxLength(200)]    
        public string DeliveryAddress { get; set; } = String.Empty;
        [Required]
        [MinLength(10),MaxLength(10)]
        public string ContactNumber { get; set; } = String.Empty;
        public string PaymentMethod { get; set; } = String.Empty;
        //Order Items
        [Required]
        public List<OrderItemDto> OrderItems{get;set;} = new List<OrderItemDto>();
    }
}