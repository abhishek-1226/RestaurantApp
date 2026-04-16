using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public OtpService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> GenerateOtpCode()
        {
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            return await Task.FromResult(otp);
        }

        public async Task<bool> VerifyOtpCode(string email, string otpCode)
        {
            var otpRecord = await _context.OtpVerifications
                .Where(o => o.Email == email && o.OtpCode == otpCode && !o.IsUsed && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                return false;
            }

            // Check expiry
            if (otpRecord.ExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            // Check max attempts
            if (otpRecord.AttemptCount >= otpRecord.MaxAttempts)
            {
                return false;
            }

            otpRecord.AttemptCount++;

            if (otpRecord.OtpCode == otpCode)
            {
                otpRecord.IsUsed = true;
                otpRecord.VerifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Mark user as verified
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    user.IsVerified = true;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return true;
            }

            await _context.SaveChangesAsync();
            return false;
        }

        public async Task SaveOtpCode(string email, string otpCode, string purpose)
        {
            // Invalidate any existing unused OTPs for this email
            var existingOtps = await _context.OtpVerifications
                .Where(o => o.Email == email && !o.IsUsed && !o.IsDeleted)
                .ToListAsync();

            foreach (var otp in existingOtps)
            {
                otp.IsDeleted = true;
                otp.DeletedAt = DateTime.UtcNow;
            }

            var otpRecord = new OtpVerificationModel
            {
                Email = email,
                OtpCode = otpCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                AttemptCount = 0
            };

            _context.OtpVerifications.Add(otpRecord);
            await _context.SaveChangesAsync();
        }

        public async Task SendOtp(string email, string otpCode)
        {
            var subject = "RestaurantApp - Your Verification Code";
            var body = $"Your OTP verification code is: {otpCode}. This code expires in 10 minutes.";
            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
