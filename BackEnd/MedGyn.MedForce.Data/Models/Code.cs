namespace MedGyn.MedForce.Data.Models
{
	public class Code
	{
		public virtual int CodeID { get; set; }
		public virtual string CodeName { get; set; }
		public virtual string CodeDescription { get; set; }
		public virtual int CodeTypeID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual bool IsRequired { get; set; }
	}
}
