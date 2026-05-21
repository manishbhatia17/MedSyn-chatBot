using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
    public class CustomerChatLogMapping : ClassMap<CustomerChatLog>
    {
        public CustomerChatLogMapping()
        {
            Table("CustomerChatLogs");

            Id(x => x.Id)
                .Column("Id")
                .GeneratedBy.Identity();

            Map(x => x.Name);

            Map(x => x.Email);

            Map(x => x.PhoneNumber);

            Map(x => x.State);

            Map(x => x.Country);

            Map(x => x.IsExistingCustomer);

            Map(x => x.CustomerId);

            Map(x => x.CreatedAt);
        }
    }
}