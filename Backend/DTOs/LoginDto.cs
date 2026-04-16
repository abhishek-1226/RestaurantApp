using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantApp.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email{ get; set; }
        [Required]
        public string Password{ get; set; }
    }
}