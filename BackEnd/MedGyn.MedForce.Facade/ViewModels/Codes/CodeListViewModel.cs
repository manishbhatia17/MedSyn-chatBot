using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CodeListViewModel : BaseListViewModel<CodeViewModel>
	{
		public CodeListViewModel(SearchCriteriaViewModel sc, List<CodeViewModel> results) : base(sc, results)
		{

		}
	}
}
