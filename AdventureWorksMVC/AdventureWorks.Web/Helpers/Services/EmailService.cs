using System.Net.Mail;
using System.Threading.Tasks;
using AdventureWorks.Web.Helpers.Settings;
using Microsoft.AspNet.Identity;

namespace AdventureWorks.Web.Helpers.Services
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            SmtpClient client = new SmtpClient();

            MailMessage mail = new MailMessage { Subject = message.Subject, Body = message.Body, IsBodyHtml = true };

#if DEBUG
            mail.To.Add(new MailAddress(AppSettings.EMailProjectAddress));
#else
            mail.To.Add(new MailAddress(message.Destination));
#endif

            //try
            //{
            return client.SendMailAsync(mail);
            //}
            //catch (SmtpException ex)
            //{
            //    //return ex.Message + ": " + ex.InnerException.Message;
            //    return Task.FromResult(0);
            //}
        }
    }
}