using System;
using System.Net.Mail;
using NLog;
using AdventureWorks.Web.Helpers.Settings;

namespace AdventureWorks.Web.Helpers.Email
{
    public static class EmailHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static string SendMail(string subject, string eMail, string name, string message)
        {
            if (string.IsNullOrEmpty(AppSettings.EMailHostName))
                return "E-mail host is not configured!";

#if DEBUG
            bool fakeEmailSend = true;
#else
            bool fakeEmailSend = AppSettings.FakeEmailSend;
#endif

            using (MailMessage mail = new MailMessage())
            {
                if (fakeEmailSend)
                {
                    mail.To.Add(string.IsNullOrEmpty(name)
                        ? new MailAddress(AppSettings.EMailProjectAddress)
                        : new MailAddress(AppSettings.EMailProjectAddress, $"{name} ({eMail})", System.Text.Encoding.UTF8));
                }
                else
                {
                    mail.To.Add(string.IsNullOrEmpty(name)
                        ? new MailAddress(eMail)
                        : new MailAddress(eMail, name, System.Text.Encoding.UTF8));

                    if (!string.IsNullOrEmpty(AppSettings.EMailBcc))
                        mail.Bcc.Add(AppSettings.EMailBcc);
                }

                mail.From = new MailAddress(AppSettings.EMailFromAddress, AppSettings.EMailFromName, System.Text.Encoding.UTF8);
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = message;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                //mail.Priority = MailPriority.High;

                using (SmtpClient client = new SmtpClient())
                {
                    if (!string.IsNullOrEmpty(AppSettings.EMailUserName))
                        client.Credentials = new System.Net.NetworkCredential(AppSettings.EMailUserName, AppSettings.EMailPassword);

                    if (AppSettings.EMailHostPort != 0)
                        client.Port = AppSettings.EMailHostPort;

                    client.Host = AppSettings.EMailHostName;

                    client.EnableSsl = AppSettings.EMailHostEnableSsl;

                    Log.Trace("Sending email {0}: {1}", eMail, subject);

                    try
                    {
                        client.Send(mail);

                        return "";
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);

                        return "Sending email failed. Contact the administrator.";
                    }
                }
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress addr = new MailAddress(email);

                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}