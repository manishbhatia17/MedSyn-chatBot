using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class CodeTypeMapping : ClassMap<CodeType>
	{
		public CodeTypeMapping()
		{
			Table("[CodeType]");
			Id(x => x.CodeTypeID).GeneratedBy.Assigned();

			Map(x => x.CodeTypeName);
			Map(x => x.IsDeleted);
			Map(x => x.LockCodes);
		}
	}
}
