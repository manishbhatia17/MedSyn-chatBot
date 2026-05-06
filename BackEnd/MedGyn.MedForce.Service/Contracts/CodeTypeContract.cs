using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CodeTypeContract
	{
		public CodeTypeContract() { }

		public CodeTypeContract(CodeType codeType)
		{
			CodeTypeID   = codeType.CodeTypeID;
			CodeTypeName = codeType.CodeTypeName;
			LockCodes    = codeType.LockCodes;
		}

		public int CodeTypeID { get; set; }
		public string CodeTypeName { get; set; }
		public bool LockCodes { get; set; }

		public CodeType ToModel()
		{
			return new CodeType()
			{
				CodeTypeID   = CodeTypeID,
				CodeTypeName = CodeTypeName,
			};
		}
	}
}
