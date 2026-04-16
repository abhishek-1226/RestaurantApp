using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace RestaurantApp.Models
{
   [Index(nameof(Email), IsUnique=true)] 
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public bool IsVerified { get; set; } = false;
        public int RoleId { get; set; }
        public RoleModel Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastOtpSentAt { get; set; }
        [Required]
        [MaxLength(10)]
        public string Status { get; set; } = "Active"; // Active, Inactive, Banned
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}