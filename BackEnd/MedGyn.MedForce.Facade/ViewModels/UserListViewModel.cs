using System.Collections.Generic;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class UserListViewModel : BaseListViewModel<UserDisplayViewModel>
	{
		public UserListViewModel(SearchCriteriaViewModel sc, List<UserDisplayViewModel> results) : base(sc, results)
		{

		}
	}
	
}
