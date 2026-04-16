using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.DTOs
{
    public class RestaurantCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        public int PinCode { get; set; }

        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }
    }
}
