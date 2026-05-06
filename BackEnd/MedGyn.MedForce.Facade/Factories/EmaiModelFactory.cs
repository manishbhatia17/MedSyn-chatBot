using Medforce.Graph.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.Factories
{
	public class EmaiModelFactory
	{
		public EmaiModelFactory() { }

		public EmailModel BuildReplyEmail(string body, string replyToId)
		{
			string cleanedBody = body.Trim('`', '\n').Replace("json\n", "");
			EmailModel replyEmail = JsonConvert.DeserializeObject<EmailModel>(cleanedBody);
			replyEmail.BCCReipiants = new List<string> { "8092035@bcc.hubspot.com" };
			replyEmail.CCRecipiants = new List<string>();
			replyEmail.Id = replyToId;
			return replyEmail;
		}

		public EmailModel BuildReplyEmailWithAttachments(string body, string replyToId, List<string> attachments)
		{

			EmailModel replyEmail = BuildReplyEmail(body, replyToId);
			replyEmail.Attachments = attachments;

			return replyEmail;
		}
	}
}
