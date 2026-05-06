using System.Collections.Generic;
using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
	public interface ICodeFacade
	{
		CodeTypeListViewModel GetAllCodeTypes(SearchCriteriaViewModel sc);
		CodeListViewModel GetCodesByType(SearchCriteriaViewModel sc, CodeTypeEnum codeType);
		bool SaveCodeTypes(List<CodeTypeViewModel> codeTypes);
		bool SaveCodes(List<CodeViewModel> codes, CodeTypeEnum codeTypeID);
	}
}
