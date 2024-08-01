using ExamCenterApp.Services.Model;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ExamCenterApp.Services
{
    public class Email_Sender : IEmail_Sender
    {
        private readonly IEmail_Configuration _configuration;
        private readonly ILogger<Email_Message> _logger;
        public Email_Sender(IEmail_Configuration configuration, ILogger<Email_Message> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void CallHangFire(Email_Message email_message, string[]from, string display_username)
        {
            BackgroundJob.Enqueue(() => Send(email_message, from, display_username)); 
        }
        public void Send(Email_Message email_message, string[] from, string display_username)
        {
            var emailMessage = new MimeMessage();
            string senderDisplayName = display_username ?? _configuration.DisplayName ?? "Email Message";
            foreach (string sender in from)
            {
                var ad = MailboxAddress.Parse(sender);
                ad.Name = senderDisplayName;
                emailMessage.From.Add(ad);
            }

            emailMessage.To.AddRange(email_message.TO_addresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            emailMessage.Subject = email_message.subject;
            BodyBuilder emailBody;
            emailBody = BuildBody(email_message.content, email_message.attachments);
            emailMessage.Body = emailBody.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(_configuration.SmtpServer, _configuration.SmtpPort, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_configuration.SmtpUsername, _configuration.SmtpPassword);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Message sending failed from {emailMessage.From} to {emailMessage.To}." +
                        $"{Environment.NewLine}{ex.Message}");
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
        public BodyBuilder BuildBody(string message, IFormFileCollection attachments = null)
        {
            BodyBuilder body = new BodyBuilder
            {
                HtmlBody = message
            };

            if (attachments != null && attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    body.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            return body;
        }
        public void SendEmail(string TO_email, string subject, string message, string user_display_name=null)
        {
            EmailAddress emailAddress = new EmailAddress() { 
                Address = TO_email
            };
            List<EmailAddress> TO_adress_list = new List<EmailAddress> {
                emailAddress
                
            };
            Email_Message email_Message = new Email_Message()
            {
                TO_addresses = TO_adress_list,
                content = message,
                subject = subject

            };
            CallHangFire(email_Message, [_configuration.SmtpUsername], _configuration.DisplayName);
        }
        public void SendEmailWithAttachment(string TO_email, string subject, string message,IFormFileCollection attachments, string user_display_name = null)
        {
            EmailAddress emailAddress = new EmailAddress()
            {
                Address = TO_email
            };
            List<EmailAddress> TO_adress_list = new List<EmailAddress> {
                emailAddress

            };
            Email_Message email_Message = new Email_Message()
            {
                TO_addresses = TO_adress_list,
                content = message,
                subject = subject,
                attachments = attachments

            };
            CallHangFire(email_Message, [_configuration.SmtpUsername], _configuration.DisplayName);
        }
    }
}
