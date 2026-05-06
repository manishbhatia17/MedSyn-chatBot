using Medforce.Graph.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface IEmailResponseFacade
	{
		Task<string> GetEmailResponseAsync(string content, string agentFunctions, string agentDirections = "");
		Task ProcessNotificationAsync(GraphWebhookChangeNotification payload);
	}
}
