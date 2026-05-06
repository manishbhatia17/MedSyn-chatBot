using Medforce.Graph.Services.Models;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medforce.Graph.Services.Interfaces
{
	public interface IEmailService
	{
		Task<IList<EmailModel>> GetEmails(string InboxId);
		Task<IList<EmailModel>> GetNewEmails(string InboxId, DateTime lastChecked);
		Task CreateDraftEmail(string InboxId, EmailModel emailModel);
		Task CreateDraftReply(string InboxId, EmailModel emailModel);
		
	}
}
