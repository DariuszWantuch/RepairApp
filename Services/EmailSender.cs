using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string _subject, string _message, string _email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("darek.wantuch@wp.pl", "Naprawa RTV/AGD");
            var subject = _subject;
            var to = new EmailAddress(_email, "Witaj");
            var plainTextContent = _message;
            var htmlContent = _message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);              

            return client.SendEmailAsync(msg);
        }
    }
}
