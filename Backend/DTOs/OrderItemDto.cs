using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantApp.DTOs
{
    public class OrderItemDto
    { //Input DTO for ordering
        [Required]
        public int MenuItemId { get; set; }
        [Required]
        [Range(1,100)]
        public int Quantity { get; set; }   
        [MaxLength(300)]
        public string? SpecialInstructions { get; set; }
    }
}