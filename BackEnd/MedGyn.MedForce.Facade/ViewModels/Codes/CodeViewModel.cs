using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CodeViewModel
	{
		public CodeViewModel() { }

		public CodeViewModel(CodeContract code)
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

		public CodeContract ToContract()
		{
			return new CodeContract()
			{
				CodeID          = CodeID,
				CodeName        = CodeName,
				CodeDescription = CodeDescription,
				IsDeleted       = IsDeleted,
			};
		}
	}
}
