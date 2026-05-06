using System;
using System.Collections.Generic;
using System.Text;

namespace Medforce.Graph.Services.Models
{
	public class EmailModel
	{
		public string Id { get; set; }
		public string From { get; set; }
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public List<string> CCRecipiants { get; set; }
		public List<string> BCCReipiants { get; set; }
		public List<string> Attachments { get; set; } = new List<string>();
	}
}
