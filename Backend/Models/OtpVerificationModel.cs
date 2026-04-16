using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace RestaurantApp.Models
{
    [Index(nameof(Email))] // Ensure unique email for OTP verification
    [Index(nameof(OtpCode))] // Index for faster lookup of OTP codes
    public class OtpVerificationModel
    {
        [Key] // Primary key for the OTP verification record
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email {get; set;}
        [Required]
        [MaxLength(6)]
        [MinLength(6)]
        public string OtpCode { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false; // To track if the OTP has been used
        public int AttemptCount { get; set; } = 0; // To track the number of verification attempts
        public int MaxAttempts { get; set; } = 3; // Maximum allowed attempts before locking out
        [MaxLength(20)]
        public string? Purpose { get; set; } // e.g., "PasswordReset", "EmailVerification"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedAt { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft delete flag
        public DateTime? DeletedAt { get; set; }
        
    }
}