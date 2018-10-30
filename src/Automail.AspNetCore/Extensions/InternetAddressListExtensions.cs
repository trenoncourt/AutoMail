using System.ComponentModel.DataAnnotations;
using MimeKit;

namespace Automail.AspNetCore.Extensions
{
    public static class InternetAddressListExtensions
    {
        public static void AddAdresses(this InternetAddressList internetAddressList, string addresses)
        {
            if (string.IsNullOrEmpty(addresses))
                return;

            var emailChecker = new EmailAddressAttribute();
            foreach (string adress in addresses.Split(';'))
            {
                if (!emailChecker.IsValid(adress))
                {
                    continue;
                }
                internetAddressList.Add(new MailboxAddress("", adress));
            }
        }
    }
}