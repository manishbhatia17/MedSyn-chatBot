using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MedGyn.MedForce.Service.Interfaces
{
    public interface IEmailService
    {
        string GetEmailTemplate(string templateName);
        Dictionary<string, string> GetEmailTemplates();
        void SendEmail(string toEmail, string subject, string body, string ccEmail = null, Dictionary<string, Stream> attachments = null);
    }
}
