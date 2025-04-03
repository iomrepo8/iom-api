using IOM.Models.ApiControllerModels;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOM.Services
{
    public class SendGridMailServices : IIdentityMessageService
    {
        private static SendGridMailServices _instance;
        private static readonly object _lock = new object();

        private SendGridMailServices() { }

        internal static SendGridMailServices Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SendGridMailServices();
                    }

                    return _instance;
                }
            }
        }

        public async Task SendAsync(IdentityMessage message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var client = new SendGridClient(EmailSettings.Instance.SendGridApiKey);
            var from = new EmailAddress(EmailSettings.Instance.EmailAccount,
                                        EmailSettings.Instance.SenderName);

            var to = new EmailAddress(message.Destination, "");

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                message.Subject,
                message.Body,
                message.Body);
            _ = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }

        public async Task SendMultipleAsync(IdentityMessage message, List<EmailAddress> recipients)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var client = new SendGridClient(EmailSettings.Instance.SendGridApiKey);
            var from = new EmailAddress(EmailSettings.Instance.EmailAccount,
                                        EmailSettings.Instance.SenderName);

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, 
                recipients, 
                message.Subject, 
                null, 
                message.Body);

           await client.SendEmailAsync(msg).ConfigureAwait(false);
        }

        public static async Task SupportInquiry(SupportEmail emailContent, List<EmailAddress> recipients, EmailAddress sender)
        {
            if (emailContent == null) throw new ArgumentNullException(nameof(emailContent));

            ISendGridClient client = new SendGridClient(EmailSettings.Instance.SendGridApiKey);

            var message = MailHelper.CreateSingleEmailToMultipleRecipients(sender,
                recipients, emailContent.Subject,
                plainTextContent: "", htmlContent: emailContent.Content);

            _ = await client.SendEmailAsync(message).ConfigureAwait(false);
        }
    }
}