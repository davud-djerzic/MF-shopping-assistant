using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MF_Shopping_Assistant.Classes
{
    internal class SendEmail
    {
        public static void SendPdfEmail(string toEmail, string pdfFilePath)
        {
            try
            {                
                string fromEmail = Connections.FromEmail;  
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUser = Connections.SmtpUsername; 
                string smtpPass = Connections.SmtpPassword; 

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Your PDF File",
                    Body = "Please find attached the PDF file.",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);

                // Attach PDF
                var attachment = new Attachment(pdfFilePath);
                mailMessage.Attachments.Add(attachment);

                // Setup SMTP client
                using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtpClient.EnableSsl = true; // Use SSL if required
                    smtpClient.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom slanja maila: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool ValidateEmail(string emailAddress)
        {
            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(emailAddress, emailRegex, RegexOptions.IgnoreCase);
        }
    }
}
