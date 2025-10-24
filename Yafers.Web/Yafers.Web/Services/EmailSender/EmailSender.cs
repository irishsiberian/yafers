using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Task = System.Threading.Tasks.Task;
using Configuration = brevo_csharp.Client.Configuration;
using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services.EmailSender
{
    public class EmailSender : IEmailSender<ApplicationUser>, IEmailSender
    {
        private readonly ILogger _logger;
        private readonly string? _apiKey;

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor,
            ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public EmailSenderOptions Options { get; } //Set with Secret Manager.

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var config = new Configuration();
            config.ApiKey.Add("api-key", Options.BrevoApiKey);

            var apiInstance = new TransactionalEmailsApi(config);

            var sendSmtpEmail = new SendSmtpEmail(
                sender: new SendSmtpEmailSender { Email = "no-reply@yafers.org", Name = "Yafers" },
                to: new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
                subject: subject,
                htmlContent: htmlMessage
            );

            try
            {
                var result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
                _logger.LogInformation($"Email to {toEmail} sent successfully via Brevo.");
            }
            catch (ApiException e)
            {
                _logger.LogError($"Brevo API error when sending email: {e.Message}");
                throw;
            }
        
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");

    }
}
