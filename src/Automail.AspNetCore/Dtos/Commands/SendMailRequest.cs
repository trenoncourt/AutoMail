using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automail.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace Automail.AspNetCore.Dtos.Commands
{
    public class SendMailCommand
    {
        /// <summary>
        /// Sender email adress.
        /// </summary>
        public string From { get; set; }

        public string FromName { get; set; }
        
        /// <summary>
        /// Recipient email address(es).
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Cc email address(es).
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// Email subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Define if mail is html.
        /// </summary>
        public bool IsHtml { get; set; }

        public IEnumerable<IFormFile> Files { get; set; }
    }

    public static class SendMailRequestExtensions
    {
        public static MimeMessage ToMimeMessage(this SendMailCommand dto, SmtpSettings settings)
        {
            var emailMessage = new MimeMessage();
            if (dto.Files != null && dto.Files.Any())
            {
                var multipart = new Multipart("mixed");
                multipart.Add(new TextPart(dto.IsHtml ? "html" : "plain") { Text = dto.Body });
                foreach (IFormFile file in dto.Files)
                {
                    Stream stream = new MemoryStream();
                    file.CopyTo(stream);
                    multipart.Add(new MimePart {Content = new MimeContent(stream), FileName = file.FileName});
                }
                emailMessage.Body = multipart;
            }
            else
            {
                emailMessage.Body = new TextPart(dto.IsHtml ? "html" : "plain") {Text = dto.Body};
            }
            emailMessage.From.Add(new MailboxAddress(dto.FromName ?? dto.From ?? settings.User, dto.From ?? settings.User));
            emailMessage.Sender = new MailboxAddress(dto.FromName ?? dto.From ?? settings.User, dto.From ?? settings.User);
            emailMessage.ReplyTo.Add(new MailboxAddress(dto.FromName ?? dto.From ?? settings.User, dto.From ?? settings.User));
            emailMessage.To.AddAdresses(dto.To);
            emailMessage.Cc.AddAdresses(dto.Cc);
            emailMessage.Subject = dto.Subject;
            return emailMessage;
        }

        public static bool IsValid(this SendMailCommand dto, SmtpSettings settings)
        {
            return !string.IsNullOrEmpty(dto.To) && (!string.IsNullOrEmpty(dto.From) || !string.IsNullOrEmpty(settings.User));
        }
    }
}