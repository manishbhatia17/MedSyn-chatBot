using MedGyn.MedForce.Common.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Medforce.Graph.Services;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Graph.Models;
using Azure.Identity;
using Microsoft.Extensions.Caching.Memory; // Add this for AuthenticationHeaderValue

// ... other using statements

namespace GraphRepository
{
	public class BaseGraphClass : GraphAuth
	{
		protected GraphServiceClient _graphClient { get; private set; }
		protected readonly string GraphUrl;

		public BaseGraphClass(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
			: base(memoryCache, appSettings)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			GraphUrl = _appSettings.GraphUrl;

			_graphClient = new GraphServiceClient(new ClientCertificateCredential(_appSettings.Realm, _appSettings.GraphClientId, CreateCertificate()));
		}
	}
}
