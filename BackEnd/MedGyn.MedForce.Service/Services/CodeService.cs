using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Constants;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class CodeService : ICodeService
	{
		private readonly ICodeRepository _codeRepository;

		public CodeService(ICodeRepository codeRepository)
		{
			_codeRepository = codeRepository;
		}

		public IEnumerable<CodeTypeContract> GetAllCodeTypes(string search, string sortCol, bool sortAsc)
		{
			return _codeRepository.GetAllCodeTypes(search, sortCol, sortAsc).Select(x => new CodeTypeContract(x));
		}

		public IEnumerable<CodeContract> GetCodesByType(CodeTypeEnum codeType, string search = null, string sortCol = null, bool sortAsc = true)
		{
			return _codeRepository.GetCodesByType(codeType, search, sortCol, sortAsc).Select(c => new CodeContract(c));
		}

		public IDictionary<int, CodeContract> GetCodeLookupByType(CodeTypeEnum codeType)
		{
			var lookup = GetCodesByType(codeType).ToDictionary(c => c.CodeID);
			if(!lookup.ContainsKey(0))
				lookup.Add(0, null);
			return lookup;
		}
		public bool SaveCodeTypes(List<CodeTypeContract> codeTypes)
		{
			return _codeRepository.SaveCodeTypes(codeTypes.Select(x => x.ToModel()).ToList());
		}
		public bool SaveCodes(List<CodeContract> codes, CodeTypeEnum codeTypeID)
		{
			var originals = _codeRepository.GetCodesByType(codeTypeID).ToDictionary(x => x.CodeID);
			foreach(var code in codes)
			{
				if(code.CodeID > 0 && originals[code.CodeID].IsRequired)
				{
					code.CodeName = originals[code.CodeID].CodeName;
				}
			}

			return _codeRepository.SaveCodes(codes.Select(x => x.ToModel()).ToList(), codeTypeID);
		}
	}
}
