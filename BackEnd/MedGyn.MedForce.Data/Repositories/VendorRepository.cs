using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Data.Repositories
{
	public class VendorRepository : IVendorRepository
	{
		private readonly ConnectionStrings _connectionStrings;
		private readonly IDbContext _dbContext;

		public VendorRepository(IOptions<ConnectionStrings> connectionStringsOptionsAccessor, IDbContext dbContext)
		{
			_connectionStrings = connectionStringsOptionsAccessor.Value;
			_dbContext = dbContext;
		}

		public IEnumerable<Vendor> GetAllVendors()
		{
			return _dbContext.Vendors;
		}
		public IEnumerable<Vendor> GetAllVendors(string search, string sortCol, bool sortAsc)
		{
			var queryText = "SELECT v.* FROM Vendor v ";
			if (!search.IsNullOrEmpty())
			{
				queryText += $@"
					LEFT JOIN Code c on c.{nameof(Code.CodeID)} = v.{nameof(Vendor.CountryCodeID)}
						WHERE v.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR v.{nameof(Vendor.VendorCustomID)} LIKE :searchTerm
						OR v.{nameof(Vendor.City)} LIKE :searchTerm
						OR c.{nameof(Code.CodeDescription)} LIKE :searchTerm
				";
			}

			var sortProp = typeof(Vendor).GetProperties().FirstOrDefault(x => x.Name.ToLower() == sortCol.ToLower())?.Name;
			if(!sortProp.IsNullOrEmpty())
				queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			var query = _dbContext.Session.CreateSQLQuery(queryText).AddEntity(typeof(Vendor));
			if (!search.IsNullOrEmpty())
			{
				query.SetString("searchTerm", $"%{search}%");
			}

			return query.List<Vendor>();
		}

		public Vendor GetVendor(int vendorID)
		{
			var model = _dbContext.Get<Vendor>(vendorID);
			if (model == null)
			{
				return new Vendor();
			}

			return model;
		}

		public Vendor SaveVendor(Vendor vendor)
		{
			_dbContext.BeginTransaction();
			_dbContext.Save(vendor);
			_dbContext.Commit();

			return vendor;
		}

		public void UpdateVendor(Vendor vendor)
		{
			_dbContext.BeginTransaction();
			_dbContext.SaveOrUpdate(vendor);
			_dbContext.Commit();
		}

		public bool ValidateVendorCustomID(int vendorID, string vendorCustomID)
		{
			return (from v in _dbContext.Vendors where v.VendorCustomID == vendorCustomID && v.VendorID != vendorID select v.VendorID).Count() == 0;
		}
	}
}
