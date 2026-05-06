using System.Collections.Generic;

namespace MedGyn.MedForce.Data.Models
{
	public class CodeType
	{
		public virtual int CodeTypeID { get; set; }
		public virtual string CodeTypeName { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual bool LockCodes { get; set; }
	}
}
