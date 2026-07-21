using System.Collections.Generic;

namespace MedGyn.MedForce.Common.Configurations
{
	public class ChatWidgetSettings
	{
		public string SharedToken { get; set; }
		public int MaxRequestsPerWindow { get; set; } = 20;
		public int WindowSeconds { get; set; } = 60;
		public List<AllowedEmbed> AllowedEmbeds { get; set; } = new List<AllowedEmbed>();
	}

	public class AllowedEmbed
	{
		public string CompanyId { get; set; }
		public string Domain { get; set; }
	}
}
