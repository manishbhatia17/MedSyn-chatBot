using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class RoleListViewModel : BaseListViewModel<RoleViewModel>
	{
		public RoleListViewModel(SearchCriteriaViewModel sc, List<RoleViewModel> results) : base(sc, results)
		{

		}
	}
}
