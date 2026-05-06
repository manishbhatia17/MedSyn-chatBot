using GraphRepository;
using MedGyn.MedForce.Common.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medforce.Graph.Services
{
	public class GraphSharePointListSearch :BaseGraphClass, Interfaces .ISharePointListSearchService
	{
		public GraphSharePointListSearch(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
			: base(memoryCache, appSettings)
		{
		}

		public async Task<List<string>> SearchSharePointList(string siteId, string listId, string query)
		{
			List<string> results = new List<string>();

			// Build the search request for SharePoint list items
			var searchRequest = new Microsoft.Graph.Models.SearchRequest
			{
				EntityTypes = new List<EntityType?> { EntityType.ListItem },
				Query = new SearchQuery
				{
					QueryString = query
				},
				Region = "US",
				// Optionally, you can restrict the search to a specific site or list using the 'AdditionalProperties'
				AdditionalData = new Dictionary<string, object>
				{
					{
						"sharepoint", new Dictionary<string, object>
						{
							{ "siteId", siteId },
							{ "listId", listId }
						}
					}
				}
			};

			var searchRequests = new List<Microsoft.Graph.Models.SearchRequest> { searchRequest };

			var response = await _graphClient.Search.Query.PostAsQueryPostResponseAsync(new Microsoft.Graph.Search.Query.QueryPostRequestBody()
			{
				Requests = searchRequests
			});

			if (response?.Value != null)
			{
				foreach (var searchResponse in response.Value)
				{
					if (searchResponse.HitsContainers != null)
					{
						foreach (var container in searchResponse.HitsContainers)
						{
							if (container.Hits != null)
							{
								foreach (var hit in container.Hits)
								{
									if (hit.Resource?.AdditionalData != null &&
										hit.Resource.AdditionalData.TryGetValue("fields", out var fieldsObj) &&
										fieldsObj is IDictionary<string, object> fieldsDict &&
										fieldsDict.TryGetValue("ProductID", out var productIdObj) &&
										productIdObj != null)
									{
										results.Add(productIdObj.ToString());
									}
								}
							}
						}
					}
				}
			}

			return results;
		}
	}
}
