namespace Automail.Api.Dtos.Requests
{
    public class SendMailRequest
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
    }
}