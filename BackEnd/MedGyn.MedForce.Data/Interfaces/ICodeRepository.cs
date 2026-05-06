using MedGyn.MedForce.Data.Models;
using System.Collections.Generic;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface ICodeRepository
	{
		IEnumerable<CodeType> GetAllCodeTypes(string search, string sortCol, bool sortAsc);
		IEnumerable<Code> GetCodesByType(CodeTypeEnum codeType, string search = null, string sortCol = null, bool sortAsc = true);
		bool SaveCodeTypes(List<CodeType> codeTypes);
		bool SaveCodes(List<Code> codes, CodeTypeEnum codeTypeID);
	}
}
