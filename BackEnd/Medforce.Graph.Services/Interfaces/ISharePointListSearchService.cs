using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medforce.Graph.Services.Interfaces
{
	public interface ISharePointListSearchService
	{
		Task<List<string>> SearchSharePointList(string siteId, string listId, string query);
	}
}
