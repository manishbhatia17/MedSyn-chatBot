using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Facade.Facades
{
	public class CodeFacade : ICodeFacade
	{
		private readonly ICodeService _codeService;
		public CodeFacade(ICodeService codeService)
		{
			_codeService = codeService;
		}
		public CodeTypeListViewModel GetAllCodeTypes(SearchCriteriaViewModel sc)
		{
			var vms = _codeService.GetAllCodeTypes(sc.Search, sc.SortColumn, sc.SortAsc).Select(x => new CodeTypeViewModel(x)).ToList();
			
			return new CodeTypeListViewModel(sc, vms);
		}

		public CodeListViewModel GetCodesByType(SearchCriteriaViewModel sc, CodeTypeEnum codeType)
		{
			var vms = _codeService.GetCodesByType(codeType, sc.Search, sc.SortColumn, sc.SortAsc).Select(x => new CodeViewModel(x)).ToList();
			
			return new CodeListViewModel(sc, vms);
		}

		public bool SaveCodeTypes(List<CodeTypeViewModel> codeTypes)
		{
			return _codeService.SaveCodeTypes(codeTypes.Select(x => x.ToContract()).ToList());
		}

		public bool SaveCodes(List<CodeViewModel> codeTypes, CodeTypeEnum codeTypeID)
		{
			return _codeService.SaveCodes(codeTypes.Select(x => x.ToContract()).ToList(), codeTypeID);
		}
	}
}
