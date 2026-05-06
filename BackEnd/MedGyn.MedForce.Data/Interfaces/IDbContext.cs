using MedGyn.MedForce.Data.Models;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Data.Interfaces
{
	public interface IDbContext
	{
		ISession Session { get; }

		ITransaction BeginTransaction();

		Task CommitAsync();
		void Commit();
		void Rollback();
		void CloseTransaction();

		T Save<T>(T obj) where T : class;
		Task<T> SaveAsync<T>(T obj) where T : class;

		void SaveOrUpdate<T>(T obj) where T : class;
		Task SaveOrUpdateAsync<T>(T obj) where T : class;
		void Update<T>(T obj) where T : class;
		Task UpdateAsync<T>(T obj) where T : class;
		Task<T> MergeAsync<T>(T obj) where T : class;
		void Delete<T>(T obj) where T : class;
		Task DeleteAsync<T>(T obj) where T : class;
		Task<T> GetAsync<T>(int id);
		T Get<T>(int id);
		IQueryable<Code> Codes { get; }
		IQueryable<CodeType> CodeTypes { get; }
		IQueryable<Customer> Customers { get; }
		IQueryable<CustomerOrderProduct> CustomerOrderProducts { get; }
		IQueryable<CustomerOrderProductFill> CustomerOrderProductFills { get; }
		IQueryable<CustomerOrderShipment> CustomerOrderShipments { get; }
		IQueryable<CustomerOrderShipmentBox> CustomerOrderShipmentBoxes { get; }
		IQueryable<CustomerShippingInfo> CustomerShippingInfo { get; }
		IQueryable<DailyCustomerOrderCount> DailyCustomerOrderCounts { get; }
		IQueryable<DailyPurchaseOrderCount> DailyPurchaseOrderCounts { get; }
		IQueryable<Product> Products { get; }
		IQueryable<ProductInventoryAdjustment> ProductInventoryAdjustments { get; }
		IQueryable<PurchaseOrder> PurchaseOrders { get; }
		IQueryable<Role> Roles { get; }
		IQueryable<SecurityKey> SecurityKeys { get; }
		IQueryable<User> Users { get; }
		IQueryable<Vendor> Vendors { get; }
		IQueryable<CustomerChatLog> CustomerChatLogs { get; }
	}
}
