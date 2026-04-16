using RestaurantApp.Models;
namespace RestaurantApp.Services
{
    public interface IEmailService
    {
      Task SendEmailAsync(string toEmail, string subject,string body);
    }
}