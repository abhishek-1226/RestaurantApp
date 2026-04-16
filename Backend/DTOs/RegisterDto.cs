using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantApp.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string Name{ get; set; }
        [Required]
        [EmailAddress]
        public string Email{ get; set; }
        [Required]
        [MinLength(6)]
        public string Password{ get; set; }
        public int? RoleId{ get; set; }
    }
}