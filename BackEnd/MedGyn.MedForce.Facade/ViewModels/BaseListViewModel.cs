using System;
using System.Collections.Generic;
using System.Linq;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class BaseListViewModel<T>
	{

		public BaseListViewModel()
		{
			Results = new List<T>();
			AllItems = new List<T>();
		}

		public BaseListViewModel(SearchCriteriaViewModel sc, IEnumerable<T> results) : this()
		{
			AllItems = results.ToList();
			TotalResults = results.Count();
			if(sc.PageSize > 0)
			{ 
				Results = AllItems.Skip(sc.Page * sc.PageSize).Take(sc.PageSize).ToList();

				Start = (sc.Page * sc.PageSize);
				End = Math.Min(TotalResults, Start + sc.PageSize);
				CurrentPage = sc.Page;

				TotalPages = (int)Math.Ceiling(TotalResults / (double)sc.PageSize);
			}
			else
			{
				Results = AllItems;
			}
		}

		public List<T> AllItems { get; set; }
		public List<T> Results { get; set; }
		public int Start { get; set; }
		public int End { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int TotalResults { get; set; }
	}
}