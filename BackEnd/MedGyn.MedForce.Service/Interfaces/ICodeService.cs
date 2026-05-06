using MedGyn.MedForce.Service.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface ICodeService
	{
		IEnumerable<CodeTypeContract> GetAllCodeTypes(string search, string sortCol, bool sortAsc);
		IEnumerable<CodeContract> GetCodesByType(CodeTypeEnum codeType, string search = null, string sortCol = null, bool sortAsc = true);
		IDictionary<int, CodeContract> GetCodeLookupByType(CodeTypeEnum codeType);
		bool SaveCodeTypes(List<CodeTypeContract> codeTypes);
		bool SaveCodes(List<CodeContract> codes, CodeTypeEnum codeTypeID);
	}
}
