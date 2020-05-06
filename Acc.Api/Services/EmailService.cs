using Acc.Api.DataAccess;
using Acc.Api.Interface;
using Acc.Api.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        private EmailTempRepo _emailRepo;
        private AppSettings App;
        public EmailService(IOptions<EmailSettings> emailSettings, IOptions<AppSettings> appSetings)
        {
            _emailSettings = emailSettings.Value;
            App = appSetings.Value;
            _emailRepo = new EmailTempRepo(App.ConnectionString);
        }

        private Output send(EmailModel mail)
        {
            Output _result = new Output();

            try
            {
                //var message = new MailMessage();
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Beri Hati")
                };
                message.To.Add(new MailAddress(mail.to));

                message.Subject = mail.subject;
                message.Body = mail.body;
                message.IsBodyHtml = true;
                //        message.Priority = MailPriority.High;
                //message.Attachments.Add(new Attachment(Server.MapPath("~/myimage.jpg")));
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                using (var smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    //smtp.SendMailAsync(message);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }

                _result.Message = "Pesan Terkirim";
                SaveEmail(mail);
            }
            catch (Exception ex)
            {
                throw ex;
                //_result.status = false;
                //_result.Message = ex.Message;
            }

            return _result;
        }

        public Output SendEmail(EmailModel eMail)
        {
            return send(eMail);
        }

        public async Task<Output> SendEmailAsync(EmailModel eMail)
        {
            Output _result = new Output();
            try
            {
                //var message = new MailMessage();
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, _emailSettings.FromEmail)
                };
                message.To.Add(new MailAddress(eMail.to));
                if (!string.IsNullOrEmpty(eMail.cc))
                {
                    message.CC.Add(new MailAddress(eMail.cc));
                }

                message.Subject = eMail.subject;
                message.Body = eMail.body;
                message.IsBodyHtml = true;
                //        message.Priority = MailPriority.High;
                //message.Attachments.Add(new Attachment(Server.MapPath("~/myimage.jpg")));
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                using (var client = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    //smtp.SendMailAsync(message);
                    //smtp.UseDefaultCredentials = true;

                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);

                    //smtp.Send(message);
                    await client.SendMailAsync(message);
                }
                eMail.from = _emailSettings.UsernameEmail;
                _result.Message = "Pesan Terkirim";
                SaveEmail(eMail);
            }
            catch (Exception ex)
            {
                throw ex;
                //_result.status = false;
                //_result.Message = ex.Message;
            }
            return _result;
        }
        private void SaveEmail(EmailModel eMail)
        {
            try
            {
                EmailModelDB dataEMail = new EmailModelDB();
                dataEMail = eMail;
                _emailRepo.Save(dataEMail);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
