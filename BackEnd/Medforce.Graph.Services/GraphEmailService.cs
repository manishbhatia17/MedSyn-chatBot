using GraphRepository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Common.Configurations;
using System.Threading.Tasks;
using Medforce.Graph.Services.Models;
using Medforce.Graph.Services.Interfaces;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph;
using System.Net.Mail;
using System.Linq;

namespace Medforce.Graph.Services
{
	public class GraphEmailService : BaseGraphClass, IEmailService
	{
		

		public GraphEmailService(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
			: base(memoryCache, appSettings)
		{

		}

		public async Task<IList<EmailModel>> GetEmails(string InboxId)
		{

			List<EmailModel> emails = new List<EmailModel>();
			var requestInfo = _graphClient.Users[InboxId].MailFolders["inbox"]
				.Messages
				.ToGetRequestInformation();

			requestInfo.QueryParameters.Add("$orderby", "receivedDateTime desc");
			requestInfo.Headers.Add("Prefer", "outlook.body-content-type='text'");

			var messages = await _graphClient.RequestAdapter.SendAsync<MessageCollectionResponse>(
				requestInfo,
				MessageCollectionResponse.CreateFromDiscriminatorValue // Factory method to create the response  
			);

			foreach (var message in messages.Value)
			{
				EmailModel email = new EmailModel()
				{
					Id = message.Id,
					To = message.ToRecipients.Count > 0 ? message.ToRecipients[0].EmailAddress.Address : string.Empty,
					From = message.From.EmailAddress.Address,
					Subject = message.Subject,
					Body = message.Body.Content
				};

				emails.Add(email);
			}

			return emails;
		}

		public async Task<IList<EmailModel>> GetNewEmails(string InboxId, DateTime lastChecked)
		{
			List<EmailModel> emails = new List<EmailModel>();
			string filter = $"receivedDateTime gt {lastChecked:o} and isRead eq false";
			string[] order = { "receivedDateTime desc" };
			string preferHeading = "outlook.body-content-type='text'";

			//var requestInfo = _graphClient.Users[InboxId].MailFolders["inbox"]
			//	.ToGetRequestInformation();
			//requestInfo.QueryParameters.Add("$orderby", "receivedDateTime desc");

			//requestInfo.QueryParameters.Add("");

			//requestInfo.Headers.Add("Prefer", "outlook.body-content-type='text'");
			var messages = await _graphClient.Users[InboxId].Messages.GetAsync((requestInfo) =>
			{
				requestInfo.QueryParameters.Filter = filter;
				requestInfo.QueryParameters.Orderby = order;
				requestInfo.Headers.Add("Prefer", preferHeading);
			});

			foreach (var message in messages.Value)
			{

				if (message.IsRead.HasValue && message.IsRead.Value == false)
				{
					EmailModel email = new EmailModel()
					{
						Id = message.Id,
						To = message.ToRecipients.Count > 0 ? message.ToRecipients[0].EmailAddress.Address : string.Empty,
						From = message.From.EmailAddress.Address,
						Subject = message.Subject,
						Body = message.Body.Content
					};
					emails.Add(email);
				}
			}
			return emails;
		}

		public async Task CreateDraftEmail(string InboxId, EmailModel emailModel)
		{
			var toRecipient = new Recipient
			{
				EmailAddress = new EmailAddress
				{
					Address = emailModel.To
				}
			};
			var message = new Message
			{
				Subject = emailModel.Subject,
				Body = new ItemBody
				{
					ContentType = BodyType.Text,
					Content = emailModel.Body
				},
				ToRecipients = new List<Recipient> { toRecipient }
			};
			var requestInfo = await _graphClient.Users[InboxId].Messages.PostAsync(message);

		}

		public async Task CreateDraftReply(string InboxId, EmailModel emailModel)
		{
			// Step 1: Create a reply draft (includes trailing/original email)
			var replyDraft = await _graphClient.Users[InboxId]
				.Messages[emailModel.Id]
				.CreateReplyAll
				.PostAsync(new Microsoft.Graph.Users.Item.Messages.Item.CreateReplyAll.CreateReplyAllPostRequestBody { Comment = "" });

			// Step 2: Get the draft message ID
			var draftId = replyDraft.Id;

			List<Recipient> ccRecipients = emailModel.CCRecipiants.Select(r => new Recipient
			{
				EmailAddress = new EmailAddress
				{
					Address = r
				}
			}).ToList();

			List<Recipient> bccRecipients = emailModel.BCCReipiants.Select(r => new Recipient
			{
				EmailAddress = new EmailAddress
				{
					Address = r
				}
			}).ToList();

			if(emailModel.Attachments.Count() > 0)
			{
				foreach(var attachment in emailModel.Attachments)
				{
					var fileAttachment = new FileAttachment
					{
						OdataType = "#microsoft.graph.fileAttachment",
						Name = System.IO.Path.GetFileName(attachment),
						ContentBytes = System.IO.File.ReadAllBytes(attachment)
					};
					await _graphClient.Users[InboxId]
						.Messages[draftId]
						.Attachments
						.PostAsync(fileAttachment);
				}
			}

			// Step 3: Prepend your reply text to the draft body
			var updatedMessage = new Message
			{
				Body = new ItemBody
				{
					ContentType = BodyType.Html,
					Content = $"{emailModel.Body}<br><br>{replyDraft.Body.Content}" // Your reply + original
				},
				CcRecipients = ccRecipients,
				BccRecipients = bccRecipients
			};

			// Step 4: Update the draft message
			await _graphClient.Users[InboxId]
				.Messages[draftId]
				.PatchAsync(updatedMessage);
		}

	}
}
