using Acc.Api.DataAccess;
using Acc.Api.Interface;
using Acc.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
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
        private IHostingEnvironment _environment;
        public EmailService(IOptions<EmailSettings> emailSettings, IOptions<AppSettings> appSetings, IHostingEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            App = appSetings.Value;
            _emailRepo = new EmailTempRepo(App.ConnectionString);
            _environment = env;
        }

        private Output send(EmailModel mail)
        {
            Output _result = new Output();

            try
            {
                //var message = new MailMessage();
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "")
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
                string AttachPath = string.Empty;
                if (!string.IsNullOrEmpty(eMail.path_attachment))
                {
                    if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                    {
                        _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    AttachPath = Path.Combine(_environment.WebRootPath, eMail.path_attachment);
                }
                else
                {
                    if (!string.IsNullOrEmpty(eMail.attachment_string))
                    {
                        HtmlToPdfOtn._environment = _environment;
                        HtmlToPdfOtn.HTML = eMail.attachment_string;
                        HtmlToPdfOtn.Subject = eMail.subject;
                        AttachPath = HtmlToPdfOtn.PathPDF();
                    }
                }
               
                //var message = new MailMessage();
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, _emailSettings.FromEmail)
                };

                var Tos = eMail.to.Split(";");
                foreach (string to in Tos)
                {
                    message.To.Add(new MailAddress(to));
                }
                //message.To.Add(new MailAddress(eMail.to));

                if (!string.IsNullOrEmpty(eMail.cc))
                {
                    var CCS = eMail.cc.Split(";");
                    foreach ( string cc in CCS)
                    {
                        message.CC.Add(new MailAddress(cc));
                    }
                    
                }

                message.Subject = eMail.subject;
                message.Body = eMail.body;
                message.IsBodyHtml = true;
                //        message.Priority = MailPriority.High;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //attachment
                if (!string.IsNullOrEmpty(AttachPath))
                {
                    //message.Attachments.Add(new Attachment(AttachPath));
                    using (Attachment data = new Attachment(AttachPath))
                    {
                        //m.Attachments.Add(data);
                        message.Attachments.Add(data);

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
                    }
                }
                else
                {
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
                }
                
                eMail.from = _emailSettings.UsernameEmail;
                _result.Message = "Pesan Terkirim";
                SaveEmail(eMail);
                if (!string.IsNullOrEmpty(AttachPath))
                {
                    if (System.IO.File.Exists(AttachPath))
                    {
                        System.IO.File.Delete(AttachPath);
                    }
                }
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
