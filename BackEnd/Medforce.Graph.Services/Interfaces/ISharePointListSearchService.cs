using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medforce.Graph.Services.Interfaces
{
	public interface ISharePointListSearchService
	{
		Task<List<string>> SearchSharePointList(string siteId, string listId, string query);
		Task<bool> ProductDocumentExistsAsync(string productId, string folder);
		Task<(byte[] Content, string FileName)?> GetProductDocumentContentAsync(string productId, string folder);
		Task<byte[]> GetFileContentAsync(string filePath);
	}
}
