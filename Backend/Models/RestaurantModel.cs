using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace RestaurantApp.Models
{
    [Index(nameof(Name))]
    public class RestaurantModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        [MaxLength(300)]
        public string Address { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string State { get; set; }
        [Required]
        [MaxLength(6)]
        public int PinCode { get; set; }
        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
        public string? Description { get; set; }
        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<MenuItemModel> MenuItems { get; set; } = new List<MenuItemModel>();
        public int? ManagerId { get; set; }
        public UserModel? Manager { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}