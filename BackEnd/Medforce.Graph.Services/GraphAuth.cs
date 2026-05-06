using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.SharedModels;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Graph.Models.Security;
using System.Linq;

namespace GraphRepository
{
	public class GraphAuth
	{
		private readonly IMemoryCache _memoryCache;
		private IConfidentialClientApplication App;
		private readonly string[] Scopes;
		protected readonly AppSettings _appSettings;
		protected readonly string _certThumbprint;

		public GraphAuth(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
		{
			_memoryCache = memoryCache;
			_appSettings = appSettings.Value;
			string realm = _appSettings.Realm;
			string clientId = _appSettings.GraphClientId;
			_certThumbprint = _appSettings.GraphCertThumbprint;

			App = ConfidentialClientApplicationBuilder.Create(clientId)
				.WithCertificate(CreateCertificate())
				.WithAuthority(new Uri("https://login.microsoftonline.com/" + realm))
				.Build();

			Scopes = new string[]
			{
				"https://graph.microsoft.com/.default"
			};
		}

		public async Task<string> GetAuthHeaderAsync()
		{
			string token = "";
			string cacheKey = "graph_token";

			if (_memoryCache.Get(cacheKey) != null)
			{
				token = _memoryCache.Get(cacheKey).ToString();
			}
			else
			{
				AuthenticationResult result = await this.App.AcquireTokenForClient(this.Scopes).ExecuteAsync();
				token = result.AccessToken;

				DateTimeOffset expirationDate = result.ExpiresOn;
				_memoryCache.Set(cacheKey, token, expirationDate);
			}


			return token;
		}

		protected X509Certificate2 CreateCertificate()
		{
			X509Certificate2 certificate = null;
			if (!_memoryCache.TryGetValue(_certThumbprint, out certificate))
			{
				
				bool validOnly = false;

				using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
				{
					certStore.Open(OpenFlags.ReadOnly);

					X509Certificate2Collection certCollection = certStore.Certificates.Find(
												X509FindType.FindByThumbprint,
												// Replace below with your certificate's thumbprint
												_certThumbprint,
												validOnly);
					// Get the first cert with the thumbprint
					certificate = certCollection.OfType<X509Certificate2>().FirstOrDefault();

					if (certificate is null)
						throw new Exception($"Certificate with thumbprint {_certThumbprint} was not found");

					DateTimeOffset expirationDate = DateTime.Now.AddDays(1);

					_memoryCache.Set(_certThumbprint, certificate, expirationDate);
				}
			}

			return certificate;
		}
	}
}