using CoreBase.Configurations;
using CoreBase.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoreBase.Services
{
    public class EmailService:IEmailService
    {
        private const string templatePath = @"EmailTemplates/{0}.html";
        private readonly EmailConfiguration _emailConfig;       

        public EmailService(IOptions<EmailConfiguration> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }


        public async Task SendTestEmail(EmailOption emailOptions)
        {
            emailOptions.Subject = UpdatePlaceHolders("Hello {{UserName}}, reset your password.", emailOptions.PlaceHolders);

            emailOptions.Body = UpdatePlaceHolders(GetEmailBody("TestEmail"), emailOptions.PlaceHolders);

            await SendEmail(emailOptions);
        }

        private async Task SendEmail(EmailOption emailOptions)
        {

            MailMessage mail = new MailMessage
            {
                Subject = emailOptions.Subject,
                Body = emailOptions.Body,
                From = new MailAddress(_emailConfig.SenderAddress, _emailConfig.SenderDisplayName),
                IsBodyHtml = _emailConfig.IsBodyHTML
            };

            foreach (var toEmail in emailOptions.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            NetworkCredential networkCredential = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);

            SmtpClient smtpClient = new SmtpClient
            {
                Host = _emailConfig.Host,
                Port = _emailConfig.Port,
                EnableSsl = _emailConfig.EnableSSL,
                UseDefaultCredentials = _emailConfig.UseDefaultCredentials,
                Credentials = networkCredential
            };
            //attachment
            //System.Net.Mail.Attachment attachment;
            //attachment = new System.Net.Mail.Attachment("c:/Users/DELL/Pictures/test.jpg");
            //mail.Attachments.Add(attachment);

            mail.BodyEncoding = Encoding.Default;

            await smtpClient.SendMailAsync(mail);
        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }

            return text;
        }
    }
}
