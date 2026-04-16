using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.DTOs
{
    public class MenuItemCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 20000.00)]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int RestaurantId { get; set; }
    }
}
