using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using NHibernate.Linq;

namespace MedGyn.MedForce.Data.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly IDbContext _dbContext;

		public CustomerRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<Customer> GetAllCustomers(string search, string sortCol, bool sortAsc, bool seeAll, int userId, bool seeDomestic, bool seeDomesticDistribution, bool seeDomesticAfaxys, bool seeInternational)
		{
			var queryText = $@"SELECT DISTINCT c.* FROM Customer c
				LEFT JOIN Code ccode on ccode.{nameof(Code.CodeID)}=c.{nameof(Customer.CountryCodeID)}
				LEFT JOIN Code statuscode on statuscode.{nameof(Code.CodeID)}=c.{nameof(Customer.CustomerStatusCodeID)}
				LEFT JOIN Code countryCode on countryCode.{nameof(Code.CodeID)}=c.{nameof(Customer.CountryCodeID)}
				 [optionalJoin] 
				WHERE 1=1
			";
			if (!search.IsNullOrEmpty())
			{
				queryText += $@"
					AND (c.{nameof(Customer.SearchField)} like :searchTerm
							OR countryCode.{nameof(Code.CodeName)} like :searchTerm
							OR countryCode.{nameof(Code.CodeDescription)} like :searchTerm)";
			}

			//put in conditions if the use is limited to seeing specific types of customers in addition to their own
			if (!seeAll)
			{
				if (seeDomestic || seeDomesticDistribution || seeDomesticAfaxys || seeInternational)
				{
					bool first = true;

					queryText += @"
						AND (";

					if (seeDomestic)
					{
						if (!first)
							queryText += @"
											OR ";
						queryText += $"c.{nameof(Customer.IsDomestic)}=1";
						first = false;
					}
					if (seeDomesticDistribution)
					{
						if (!first)
							queryText += @"
											OR ";
						queryText += $"c.{nameof(Customer.IsDomesticDistributor)}=1";
						first = false;
					}
					if (seeDomesticAfaxys)
					{
						if (!first)
							queryText += @"
											OR ";
						queryText += $"c.{nameof(Customer.IsDomesticAfaxys)}=1";
						first = false;
					}
					if (seeInternational)
					{
						if (!first)
							queryText += @"
											OR ";
						queryText += $"c.{nameof(Customer.IsInternational)}=1";
						first = false;
					}
					queryText += @"
							)";
				}
				else
				{
					//user is only allowed to see their own
					queryText = queryText.Replace("[optionalJoin]", $@"LEFT JOIN CustomerShippingInfo csi ON csi.{nameof(CustomerShippingInfo.CustomerID)} = c.{nameof(Customer.CustomerID)}");
					queryText += $@"
					AND csi.{nameof(CustomerShippingInfo.RepUserID)} = :userId
				";
				}
			}

			queryText = queryText.Replace("[optionalJoin]", "");
			var sortProp = typeof(Customer).GetProperties().FirstOrDefault(x => x.Name.ToLower() == sortCol.ToLower())?.Name;
			if(!sortProp.IsNullOrEmpty())
				queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

            
            var query = _dbContext.Session.CreateSQLQuery(queryText).AddEntity(typeof(Customer));
			if (!search.IsNullOrEmpty())
			{
				query.SetString("searchTerm", $"%{search}%");
			}

			if (!seeAll && !seeDomestic && !seeDomesticDistribution && !seeDomesticAfaxys && !seeInternational)
			{
				query.SetInt32("userId", userId);
			}

			return query.List<Customer>();
		}

		public IEnumerable<CustomerShippingInfo> GetCustomerShippingInfo(int customerID)
		{
			return _dbContext.CustomerShippingInfo.Where(x => x.CustomerID == customerID) ;
		}

		public Customer GetCustomer(int customerID)
		{
			var model = _dbContext.Get<Customer>(customerID);
			if (model == null)
			{
				return new Customer();
			}

			return model;
		}

		public Customer GetCustomerByEmail(string email)
		{
			var model = _dbContext.Customers.Where(w => w.PrimaryEmail == email).FirstOrDefault();
			if (model == null)
			{
				return new Customer();
			}

			return model;
		}
        public async Task<Customer>  GetCustomerByEmailAsync(string email)
		{
			var model = await _dbContext.Customers.Where(w => w.PrimaryEmail == email).FirstOrDefaultAsync();
			if (model == null)
			{
				return new Customer();
			}

			return model;
		}

		public Customer SaveCustomer(Customer customer)
		{
			_dbContext.BeginTransaction();
			_dbContext.Save(customer);
			_dbContext.Commit();

			return customer;
		}

		public void UpdateCustomer(Customer customer)
		{
			_dbContext.BeginTransaction();
			_dbContext.SaveOrUpdate(customer);
			_dbContext.Commit();
		}

		//Create a new customer shipping info record
		public void SaveCustomerShippingInfo(CustomerShippingInfo shippingInfo)
		{
            _dbContext.BeginTransaction();
            _dbContext.Save(shippingInfo);
            _dbContext.Commit();
        }

		//update an existing customer shipping info record
		public void UpdateCustomerShippingInfo(CustomerShippingInfo shippingInfo)
		{
            _dbContext.BeginTransaction();
            _dbContext.SaveOrUpdate(shippingInfo);
            _dbContext.Commit();
        }

		public void SaveCustomerShippingInfo(List<CustomerShippingInfo> shippingInfo)
		{
			if(!shippingInfo.Any()) {
				return;
			}

			List<List<CustomerShippingInfo>> shippingInfoBatches = new List<List<CustomerShippingInfo>>();
			List<CustomerShippingInfo> shippingInfoBatch = new List<CustomerShippingInfo>();
			foreach (CustomerShippingInfo csi in shippingInfo)
			{
				shippingInfoBatch.Add(csi);
				if (shippingInfoBatch.Count == 50)
				{
					shippingInfoBatches.Add(shippingInfoBatch);
					shippingInfoBatch = new List<CustomerShippingInfo>();
				}
			}
            if (shippingInfoBatch.Count > 0) { shippingInfoBatches.Add(shippingInfoBatch); }

			foreach(List<CustomerShippingInfo> lcsi in shippingInfoBatches)
            {
				var properties = typeof(CustomerShippingInfo).GetProperties().Select(p => p.Name);

				var tempTableName = $"TempCustomerShippingInfo{Guid.NewGuid().ToString().Replace("-", "")}";
				var queryText = $@"
				DROP TABLE IF EXISTS #{tempTableName}
				SELECT * INTO #{tempTableName} FROM CustomerShippingInfo where 1 = 0
				SET IDENTITY_INSERT #{tempTableName} ON
				INSERT INTO #{tempTableName} ({string.Join(',', properties)}) VALUES
			";

				for (var i = 0; i < lcsi.Count; i++)
				{
					if (i > 0) { queryText += ","; }

					var props = properties.Select(x => $":{x}{i}");
					queryText += $"({string.Join(',', props)})";
				}

				queryText += $@"
				MERGE CustomerShippingInfo as t
					USING(SELECT * FROM #{tempTableName}) as s
						ON s.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = t.{nameof(CustomerShippingInfo.CustomerShippingInfoID)}
					WHEN MATCHED THEN
						UPDATE SET
							 {nameof(CustomerShippingInfo.Name)}                      = s.{nameof(CustomerShippingInfo.Name)}
							,{nameof(CustomerShippingInfo.Address)}                   = s.{nameof(CustomerShippingInfo.Address)}
							,{nameof(CustomerShippingInfo.Address2)}                  = s.{nameof(CustomerShippingInfo.Address2)}
							,{nameof(CustomerShippingInfo.City)}                      = s.{nameof(CustomerShippingInfo.City)}
							,{nameof(CustomerShippingInfo.StateCodeID)}               = s.{nameof(CustomerShippingInfo.StateCodeID)}
							,{nameof(CustomerShippingInfo.ZipCode)}                   = s.{nameof(CustomerShippingInfo.ZipCode)}
							,{nameof(CustomerShippingInfo.CountryCodeID)}             = s.{nameof(CustomerShippingInfo.CountryCodeID)}
							,{nameof(CustomerShippingInfo.RepUserID)}                 = s.{nameof(CustomerShippingInfo.RepUserID)}
							,{nameof(CustomerShippingInfo.ShipCompany1CodeID)}        = s.{nameof(CustomerShippingInfo.ShipCompany1CodeID)}
							,{nameof(CustomerShippingInfo.ShipCompany1AccountNumber)} = s.{nameof(CustomerShippingInfo.ShipCompany1AccountNumber)}
							,{nameof(CustomerShippingInfo.ShipCompany2CodeID)}        = s.{nameof(CustomerShippingInfo.ShipCompany2CodeID)}
							,{nameof(CustomerShippingInfo.ShipCompany2AccountNumber)} = s.{nameof(CustomerShippingInfo.ShipCompany2AccountNumber)}
							,{nameof(CustomerShippingInfo.SearchField)}               = s.{nameof(CustomerShippingInfo.SearchField)}
							,{nameof(CustomerShippingInfo.IsDisabled)}                = s.{nameof(CustomerShippingInfo.IsDisabled)}
							,{nameof(CustomerShippingInfo.IsDeleted)}                 = s.{nameof(CustomerShippingInfo.IsDeleted)}
					WHEN NOT MATCHED BY TARGET THEN
						INSERT VALUES (
							 s.{nameof(CustomerShippingInfo.CustomerID)}
							,s.{nameof(CustomerShippingInfo.Name)}
							,s.{nameof(CustomerShippingInfo.Address)}
							,s.{nameof(CustomerShippingInfo.City)}
							,s.{nameof(CustomerShippingInfo.StateCodeID)}
							,s.{nameof(CustomerShippingInfo.ZipCode)}
							,s.{nameof(CustomerShippingInfo.CountryCodeID)}
							,s.{nameof(CustomerShippingInfo.RepUserID)}
							,s.{nameof(CustomerShippingInfo.ShipCompany1CodeID)}
							,s.{nameof(CustomerShippingInfo.ShipCompany1AccountNumber)}
							,s.{nameof(CustomerShippingInfo.ShipCompany2CodeID)}
							,s.{nameof(CustomerShippingInfo.ShipCompany2AccountNumber)}
							,s.{nameof(CustomerShippingInfo.Address2)}
							,s.{nameof(CustomerShippingInfo.SearchField)}
							,s.{nameof(CustomerShippingInfo.IsDisabled)}
							,s.{nameof(CustomerShippingInfo.IsDeleted)}
						);
			";

				var query = _dbContext.Session.CreateSQLQuery(queryText);

				for (var i = 0; i < lcsi.Count; i++)
				{
					query.SetInt32($"CustomerShippingInfoID{i}", lcsi[i].CustomerShippingInfoID);
					query.SetInt32($"CustomerID{i}", lcsi[i].CustomerID);
					query.SetString($"Name{i}", lcsi[i].Name);
					query.SetString($"Address{i}", lcsi[i].Address);
					query.SetString($"Address2{i}", lcsi[i].Address2);
					query.SetString($"City{i}", lcsi[i].City);
					query.SetParameter<int?>($"StateCodeID{i}", lcsi[i].StateCodeID);
					query.SetString($"ZipCode{i}", lcsi[i].ZipCode);
					query.SetInt32($"CountryCodeID{i}", lcsi[i].CountryCodeID);
					query.SetInt32($"RepUserID{i}", lcsi[i].RepUserID);
					query.SetParameter<int?>($"ShipCompany1CodeID{i}", lcsi[i].ShipCompany1CodeID);
					query.SetString($"ShipCompany1AccountNumber{i}", lcsi[i].ShipCompany1AccountNumber);
					query.SetParameter<int?>($"ShipCompany2CodeID{i}", lcsi[i].ShipCompany2CodeID);
					query.SetString($"ShipCompany2AccountNumber{i}", lcsi[i].ShipCompany2AccountNumber);
					query.SetString($"SearchField{i}", lcsi[i].SearchField);
					query.SetBoolean($"IsDisabled{i}", lcsi[i].IsDisabled);
					query.SetBoolean($"IsDeleted{i}", lcsi[i].IsDeleted.HasValue ? lcsi[i].IsDeleted.Value : false);
				}

				query.ExecuteUpdate();
			}			
		}

		public void DeleteCustomerShippingInfo(CustomerShippingInfo shippingInfo)
        {
			var session = _dbContext.Session;
			if (session.IsOpen)
				session.Clear();

			var queryText = @"UPDATE CustomerShippingInfo SET IsDeleted = 1 WHERE CustomerShippingInfoID = :customerShippingInfoId";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32($"customerShippingInfoId", shippingInfo.CustomerShippingInfoID);

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();
		}

		public bool ValidateCustomerCustomID(int customerID, string customerCustomID)
		{
			return (from c in _dbContext.Customers where c.CustomerCustomID == customerCustomID && c.CustomerID != customerID select c.CustomerID).Count() == 0;
		}


	}
}
