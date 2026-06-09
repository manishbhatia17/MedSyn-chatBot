using GraphRepository;
using MedGyn.MedForce.Common.Configurations;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medforce.Graph.Services
{
	public class GraphSharePointListSearch :BaseGraphClass, Interfaces .ISharePointListSearchService
	{
		public GraphSharePointListSearch(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
			: base(memoryCache, appSettings)
		{
		}

		public async Task<string> GetProductDocumentUrlAsync(string productId, string folder)
		{
			var site = await _graphClient.Sites[_appSettings.SharePointAcademySite].GetAsync();
			if (site == null) return null;

			var folderPath = $"/sites/MedGynAcademy/Shared Documents/{folder}";

			var items = await _graphClient.Sites[site.Id].Lists["Shared Documents"].Items
				.GetAsync(config =>
				{
					config.QueryParameters.Expand = new[] { "fields($select=ProductID,FileLeafRef,FileDirRef)" };
					config.QueryParameters.Filter = $"fields/FileDirRef eq '{folderPath}'";
					config.QueryParameters.Top = 500;
				});

			if (items?.Value == null) return null;

			var match = items.Value.FirstOrDefault(item =>
			{
				var fields = item.Fields?.AdditionalData;
				if (fields == null) return false;
				fields.TryGetValue("ProductID", out var productIds);
				return productIds?.ToString()?.Contains(productId) == true;
			});

			if (match?.Fields?.AdditionalData == null) return null;

			match.Fields.AdditionalData.TryGetValue("FileLeafRef", out var fileName);
			if (fileName == null) return null;

			var encodedFileName = Uri.EscapeDataString(fileName.ToString());
			return $"https://netorgft3403149.sharepoint.com{folderPath}/{encodedFileName}";
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
