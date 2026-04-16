using RestaurantApp.Models;
namespace RestaurantApp.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpCode();
        Task<bool>VerifyOtpCode(string email,string otpCode);
        Task SaveOtpCode(string email,string otpCode,string purpose);
        Task SendOtp(string email,string otpCode);
    }
}