using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RouteX.UAF.LogicLayer.Services
{
    public class EmailService
    {
        private const string SmtpUser = "trackotesting@gmail.com";  
        private const string SmtpPass = "mirudwjdtramjhmi";         
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;

        // Send OTP Email 
        public async Task SendOtpEmailAsync(string toEmail, string otpCode, string studentName)
        {
            try
            {
                // Set up SMTP client
                using (var client = new SmtpClient(SmtpHost, SmtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
                    client.EnableSsl = true;

                    // Compose the OTP email
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(SmtpUser, "RouteX UAF Transit"),
                        Subject = "Verify your RouteX UAF Account",
                        Body = $@"
                            <div style='font-family: Arial, sans-serif; text-align: center; color: #333;'>
                                <h2>Welcome to RouteX UAF, {studentName}!</h2>
                                <p>To complete your registration and access live campus tracking, please use the following One-Time Password (OTP):</p>
                                <h1 style='color: #d35400; letter-spacing: 3px;'>{otpCode}</h1>
                                <p>This code will expire in 10 minutes.</p>
                            </div>",
                        IsBodyHtml = true
                    };

                    // Add recipient email address
                    mailMessage.To.Add(toEmail);

                    // Send the email asynchronously
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send OTP Email: {ex.Message}");
            }
        }

    }
}