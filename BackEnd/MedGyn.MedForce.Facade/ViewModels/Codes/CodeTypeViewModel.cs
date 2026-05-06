using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CodeTypeViewModel
	{
		public CodeTypeViewModel() { }
		public CodeTypeViewModel(CodeTypeContract codeType)
		{
			CodeTypeID   = codeType.CodeTypeID;
			CodeTypeName = codeType.CodeTypeName;
			LockCodes = codeType.LockCodes;
		}

		public int CodeTypeID { get; set; }
		public string CodeTypeName { get; set; }
		public bool LockCodes { get; set; }

		public CodeTypeContract ToContract()
		{
			return new CodeTypeContract()
			{
				CodeTypeID   = CodeTypeID,
				CodeTypeName = CodeTypeName,
			};
		}
	}
}
