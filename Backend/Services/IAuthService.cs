using RestaurantApp.Models;
namespace RestaurantApp.Services
{
public interface IAuthService
    {
        Task<string> GenerateJwtToken(UserModel user);
        Task<bool> VerifyPassword(string enteredPassword, string storedHash);
        Task<string> HashPassword(string password);
       
    }
}