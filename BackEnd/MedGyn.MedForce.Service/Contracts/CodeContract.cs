using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CodeContract
	{
		public CodeContract() { }

		public CodeContract(Code code)
		{
			CodeID          = code.CodeID;
			CodeName        = code.CodeName;
			CodeDescription = code.CodeDescription;
			CodeTypeID      = code.CodeTypeID;
			IsDeleted       = code.IsDeleted;
			IsRequired      = code.IsRequired;
		}

		public int CodeID { get; set; }
		public string CodeName { get; set; }
		public string CodeDescription { get; set; }
		public int CodeTypeID { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsRequired { get; set; }

		public Code ToModel()
		{
			return new Code()
			{
				CodeID          = CodeID,
				CodeName        = CodeName,
				CodeDescription = CodeDescription,
				IsDeleted       = IsDeleted,
			};
		}
	}
}
