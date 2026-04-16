using Microsoft.Extensions.Logging;

namespace RestaurantApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Stub implementation - logs email to console for development/testing
            // In production, wire this up to an SMTP service (SendGrid, Mailgun, etc.)
            _logger.LogInformation("========== EMAIL SENT ==========");
            _logger.LogInformation("To: {ToEmail}", toEmail);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", body);
            _logger.LogInformation("================================");

            await Task.CompletedTask;
        }
    }
}
