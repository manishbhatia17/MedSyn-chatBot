namespace MedGyn.MedForce.Facade.ViewModels
{
	public class SearchCriteriaViewModel
	{
		public string SortColumn { get; set; }
		public bool SortAsc { get; set; }
		public string Search { get; set; }
		public int PageSize { get; set; }
		public int Page { get; set; }
		public string productCustomID { get; set; }
	}
}
