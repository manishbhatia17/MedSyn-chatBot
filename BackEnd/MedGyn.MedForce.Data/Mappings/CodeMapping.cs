using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	class CodeMapping : ClassMap<Code>
	{
		public CodeMapping()
		{
			Table("[Code]");
			Id(x => x.CodeID)
				.Column("CodeId")
				.GeneratedBy.Identity();

			Map(x => x.CodeName);
			Map(x => x.CodeDescription);
			Map(x => x.IsDeleted);
			Map(x => x.CodeTypeID);
			Map(x => x.IsRequired);
		}
	}
}
