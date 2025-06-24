using MailKit.Net.Smtp;
using MimeKit;

namespace blogpost_website_api.Utility
{
    public class EmailService
    {
        private readonly static string smtpServer = "smtp.gmail.com";
        private readonly static int smtpPort = 587;
        private readonly static string smtpUser = "youremail@gmail.com";
        private readonly static string smtpPass = "your-app-password"; // Use App Password for Gmail

        public static async Task SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(smtpUser));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = messageBody
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
