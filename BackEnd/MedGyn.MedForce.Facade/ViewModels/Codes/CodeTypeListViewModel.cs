using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CodeTypeListViewModel : BaseListViewModel<CodeTypeViewModel>
	{
		public CodeTypeListViewModel(SearchCriteriaViewModel sc, List<CodeTypeViewModel> results) : base(sc, results)
		{

		}
	}
}
