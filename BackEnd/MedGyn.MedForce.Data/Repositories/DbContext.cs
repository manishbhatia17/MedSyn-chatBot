using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using NHibernate;
using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Exceptions;

namespace MedGyn.MedForce.Data.Repositories
{
	public class DbContext : IDbContext
	{
		private readonly ISession _session;
		private ITransaction _transaction;
		private bool _inTransaction;

		public ISession Session => _session;

		public IQueryable<Code> Codes => _session.Query<Code>();
		public IQueryable<CodeType> CodeTypes => _session.Query<CodeType>();
		public IQueryable<Customer> Customers => _session.Query<Customer>();
		public IQueryable<CustomerOrderProduct> CustomerOrderProducts => _session.Query<CustomerOrderProduct>();
		public IQueryable<CustomerOrderProductFill> CustomerOrderProductFills => _session.Query<CustomerOrderProductFill>();
		public IQueryable<CustomerOrderShipment> CustomerOrderShipments => _session.Query<CustomerOrderShipment>();
		public IQueryable<CustomerOrderShipmentBox> CustomerOrderShipmentBoxes => _session.Query<CustomerOrderShipmentBox>();
		public IQueryable<CustomerShippingInfo> CustomerShippingInfo => _session.Query<CustomerShippingInfo>();
		public IQueryable<DailyCustomerOrderCount> DailyCustomerOrderCounts => _session.Query<DailyCustomerOrderCount>();
		public IQueryable<DailyPurchaseOrderCount> DailyPurchaseOrderCounts => _session.Query<DailyPurchaseOrderCount>();
		public IQueryable<Product> Products => _session.Query<Product>();
		public IQueryable<ProductInventoryAdjustment> ProductInventoryAdjustments => _session.Query<ProductInventoryAdjustment>();
		public IQueryable<PurchaseOrder> PurchaseOrders => _session.Query<PurchaseOrder>();
		public IQueryable<Role> Roles => _session.Query<Role>();
		public IQueryable<SecurityKey> SecurityKeys => _session.Query<SecurityKey>();
		public IQueryable<User> Users => _session.Query<User>();
		public IQueryable<Vendor> Vendors => _session.Query<Vendor>();
		public IQueryable<CustomerChatLog> CustomerChatLogs => _session.Query<CustomerChatLog>();


		public DbContext(ISession session)
		{
			_session = session;
		}

		public ITransaction BeginTransaction()
		{
			if (_inTransaction)
			{
				throw new InvalidOperationException("Transaction already running; cannot start another.");
			}
			_inTransaction = true;
			_transaction = _session.BeginTransaction();
			return _transaction;
		}

		public async Task CommitAsync()
		{
			await _transaction.CommitAsync();
			_inTransaction = false;
		}

		public void Commit()
		{
			_transaction.Commit();
			_inTransaction = false;
		}

		public void Rollback()
		{
			_transaction.Rollback();
		}

		public void CloseTransaction()
		{
			if (_transaction != null)
			{
				_transaction.Dispose();
				_transaction = null;
				_inTransaction = false;
			}
		}

		public T Save<T>(T obj) where T : class
		{
			return _session.Save(obj) as T;
		}

		public async Task<T> SaveAsync<T>(T obj) where T : class
		{
			return await _session.SaveAsync(obj) as T;
		}

		public void SaveOrUpdate<T>(T obj) where T : class
		{
			_session.SaveOrUpdate(obj);
		}

		public async Task SaveOrUpdateAsync<T>(T obj) where T : class
		{
			await _session.SaveOrUpdateAsync(obj);
		}

		public void Update<T>(T obj) where T : class
		{
			_session.Update(obj);
		}

		public async Task<T> MergeAsync<T>(T obj) where T : class
		{
			return await _session.MergeAsync(obj);
		}

		public async Task UpdateAsync<T>(T obj) where T : class
		{
			await _session.UpdateAsync(obj);
		}

		public void Delete<T>(T obj) where T : class
		{
			_session.Delete(obj);
		}

		public async Task DeleteAsync<T>(T obj) where T : class
		{
			await _session.DeleteAsync(obj);
		}
		public async Task<T> GetAsync<T>(int id)
		{
			try
			{
				return await _session.GetAsync<T>(id);
			}
			catch (GenericADOException)
			{
				return default;
			}
		}

		public T Get<T>(int id)
		{
			return _session.Get<T>(id);
		}
	}
}
