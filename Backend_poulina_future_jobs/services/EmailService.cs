using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend_poulina_future_jobs.Services
{
    public interface IEmailService
    {
        Task<bool> EnvoyerEmailAsync(string destinataire, string sujet, string corps, bool estHtml = false);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnvoyerEmailAsync(string destinataire, string sujet, string corps, bool estHtml = false)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:Server"];
                var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
                var smtpUsername = _configuration["EmailSettings:Username"];
                var smtpPassword = _configuration["EmailSettings:Password"];
                var emailFrom = _configuration["EmailSettings:Sender"];
                var emailFromName = _configuration["EmailSettings:SenderName"];

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 10000 // 10 second timeout
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(emailFrom, emailFromName),
                    Subject = sujet,
                    Body = corps,
                    IsBodyHtml = estHtml,
                    Priority = MailPriority.Normal
                };

                message.To.Add(destinataire);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Destinataire}", destinataire);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error sending email to {Destinataire}. Status: {StatusCode}",
                    destinataire, smtpEx.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error sending email to {Destinataire}", destinataire);
                return false;
            }

        }
    }
}
