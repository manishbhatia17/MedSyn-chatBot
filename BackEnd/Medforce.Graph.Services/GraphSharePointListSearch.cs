using GraphRepository;
using MedGyn.MedForce.Common.Configurations;
using System;
using System.IO;
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

		private async Task<string> GetDriveIdAsync()
		{
			if (_graphClient == null) return null;

			var site = await _graphClient.Sites[_appSettings.SharePointAcademySite].GetAsync();
			if (site == null) return null;

			var drive = await _graphClient.Sites[site.Id].Drive.GetAsync();
			return drive?.Id;
		}

		private async Task<(string DriveId, DriveItem Item)?> FindBrochureItemAsync(string productId, string folder)
		{
			var driveId = await GetDriveIdAsync();
			if (driveId == null) return null;

			// FileDirRef is not indexed and is blank on these items, so it can't be used to
			// filter server-side. Instead, list the children of the target folder directly.
			var items = await _graphClient.Drives[driveId].Root
				.ItemWithPath(folder).Children
				.GetAsync(config =>
				{
					config.QueryParameters.Expand = new[] { "listItem($expand=fields($select=ProductID))" };
					config.QueryParameters.Top = 999;
				});

			if (items?.Value == null) return null;

			var match = items.Value.FirstOrDefault(item =>
			{
				var fields = item.ListItem?.Fields?.AdditionalData;
				if (fields == null) return false;
				fields.TryGetValue("ProductID", out var productIds);
				return productIds?.ToString()?.Contains(productId) == true;
			});

			if (match?.Id == null) return null;

			return (driveId, match);
		}

		public async Task<byte[]> GetFileContentAsync(string filePath)
		{
			var driveId = await GetDriveIdAsync();
			if (driveId == null) return null;

			using var stream = await _graphClient.Drives[driveId].Root.ItemWithPath(filePath).Content.GetAsync();
			if (stream == null) return null;

			using var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			return memoryStream.ToArray();
		}

		public async Task<bool> ProductDocumentExistsAsync(string productId, string folder)
		{
			var match = await FindBrochureItemAsync(productId, folder);
			return match != null;
		}

		public async Task<(byte[] Content, string FileName)?> GetProductDocumentContentAsync(string productId, string folder)
		{
			var match = await FindBrochureItemAsync(productId, folder);
			if (match == null) return null;

			var (driveId, item) = match.Value;

			using var stream = await _graphClient.Drives[driveId].Items[item.Id].Content.GetAsync();
			if (stream == null) return null;

			using var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);

			return (memoryStream.ToArray(), item.Name);
		}

		public async Task<List<string>> SearchSharePointList(string siteId, string listId, string query)
		{
			if (_graphClient == null) return new List<string>();

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
