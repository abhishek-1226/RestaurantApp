using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace RestaurantApp.Models
{
    [Index(nameof(RestaurantId))] // This creates a database index on the Name column to improve query performance when searching by name.
    public class MenuItemModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        [Range(0.01, 20000.00)]
        public decimal Price { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}