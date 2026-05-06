using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Antlr.Runtime;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IHostingEnvironment _webHostEnvironment;

		private const string tmLogo = "TM_Logo.png";
		private const string medGynLogo = "Logo.png";
		private const string tmEnv = "PROD-TM";

		public EmailService(IOptions<EmailSettings> emailSettingsOptionsAccessor, IHostingEnvironment webHostEnvironment)
        {
            _emailSettings = emailSettingsOptionsAccessor.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetEmailTemplate(string templateName)
        {
            var templatePath = _webHostEnvironment.WebRootPath
                               + Path.DirectorySeparatorChar
                               + "templates"
                               + Path.DirectorySeparatorChar
                               + templateName;

            var template = "";
            if(string.IsNullOrEmpty(templatePath))
            {
                return template;
            }

            using (var stream = File.OpenText(templatePath))
            {
                template = stream.ReadToEnd();
            }

            return template;
        }

        public Dictionary<string, string> GetEmailTemplates()
        {
            var templates = new Dictionary<string, string>();

            return templates;
        }

        public void SendEmail(string toEmail, string subject, string body, string ccEmail = null, Dictionary<string, Stream> attachments = null)
        {
            var fromAddress = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromDisplayName);
            //var toAddress = new MailAddress(toEmail);
            var email = new MailMessage();
            if(toEmail.Contains(';'))
            {
                foreach(var toAddress in toEmail.Split(';'))
                {
                    if (toAddress != "")
                    {
                        email.To.Add(toAddress);
                    }
                }
            }
            else
            {
                email.To.Add(toEmail);
            }
            email.From = fromAddress;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(ccEmail))
            {
                var ccAddress = new MailAddress(ccEmail);
                email.CC.Add(ccAddress);
            }

            if (attachments != null)
            {
                foreach (var attachmentName in attachments.Keys)
                {
                    email.Attachments.Add(new Attachment(attachments[attachmentName], attachmentName));
                }
            }

            string logoPath = _webHostEnvironment.EnvironmentName == tmEnv ? tmLogo : medGynLogo;

			var companyLogoPath = _webHostEnvironment.WebRootPath
                                                     + Path.DirectorySeparatorChar
                                                     + "images"
                                                     + Path.DirectorySeparatorChar
                                                     + logoPath;

            var bytes = File.ReadAllBytes(companyLogoPath);
            using (var logoStream = new MemoryStream(bytes))
            {
                AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

                LinkedResource headerImage = new LinkedResource(logoStream, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "companyLogo",
                    ContentType = new ContentType("image/png")
                };

                av.LinkedResources.Add(headerImage);
                email.AlternateViews.Add(av);

                logoStream.Position = 0;

                Execute(email);
            }
        }

        private void Execute(MailMessage mailMessage)
        {
            SmtpClient smtp = new SmtpClient(_emailSettings.Domain, _emailSettings.Port);

            //System.ComponentModel.AsyncCompletedEventArgs args = null;
            //smtp.SendCompleted += new SendCompletedEventHandler(SMTP_SendComplete);
            
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //string token = "emailMsg";
            //smtp.SendAsync(mailMessage, token);
            //smtp.SendMailAsync(mailMessage);
            smtp.Send(mailMessage);

        }

        private static void SMTP_SendComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //string token = (string)e.UserState;

            if (e.Error != null)
            {
                //MessageBox.Show(e.Error.ToString());
            }
            else
            {
                //MessageBox.Show("Message sent.");
            }
            return;
        }
    }
}
