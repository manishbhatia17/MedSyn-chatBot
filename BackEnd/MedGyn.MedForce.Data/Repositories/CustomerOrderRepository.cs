using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHibernate.Engine;
using NHibernate.Event.Default;

namespace MedGyn.MedForce.Data.Repositories
{
	public class CustomerOrderRepository : ICustomerOrderRepository
	{
		private readonly IDbContext _dbContext;
        private readonly ConnectionStrings _connectionStrings;

        public CustomerOrderRepository(IDbContext dbContext, IOptions<ConnectionStrings> connectionStringsOptionsAccessor)
		{
			_dbContext = dbContext;
			_connectionStrings = connectionStringsOptionsAccessor.Value;
		}

		public Task<IList<dynamic>> GetBackOrderList(string search, string sortCol, string searchProductID, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{

			var queryText = $@"
				Select co.{nameof(CustomerOrder.SubmitDate)},
						co.{nameof(CustomerOrder.PONumber)},
						co.{nameof(CustomerOrder.CustomerOrderCustomID)},
						c.{nameof(Customer.CustomerCustomID)},
						c.{nameof(Customer.CustomerName)},
						p.{nameof(Product.ProductCustomID)}, 
						bol.backorder Quantity, 
						p.{nameof(Product.ProductName)} ,
					   cp.{nameof(CustomerOrderProduct.CustomerOrderID)},
						cp.{nameof(CustomerOrderProduct.Price)},
						co.{nameof(CustomerOrder.IsFilling)},
						co.{nameof(CustomerOrder.FilledBy)}
					 from 
					[dbo].[CustomerOrder] co join 
					(
						select coi.{nameof(CustomerOrder.CustomerOrderID)},cop.{nameof(Product.ProductID)},
						( sum( cop.{nameof(CustomerOrderProduct.Quantity)}) -sum(isNUll(copf.{nameof(CustomerOrderProductFill.QuantityPacked)},0))) backorder,
						dateadd(DAY,0, datediff(day,0, coi.{nameof(CustomerOrder.SubmitDate)})) day
						from [dbo].[CustomerOrderProduct] cop 
						left join (select sum({nameof(CustomerOrderProductFill.QuantityPacked)}) quantityPacked,{nameof(CustomerOrderProductFill.CustomerOrderProductID)} 
						from  [dbo].[CustomerOrderProductFill] 
						group by {nameof(CustomerOrderProductFill.CustomerOrderProductID)}) copf
						on cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)}
						left join [dbo].[CustomerOrder] coi on cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = coi.{nameof(CustomerOrder.CustomerOrderID)}
						and coi.{nameof(CustomerOrder.SubmitDate)} is not null						
						group by  dateadd(DAY,0, datediff(day,0, coi.{nameof(CustomerOrder.SubmitDate)})),coi.CustomerOrderID,cop.{nameof(Product.ProductID)}
						having ( max(cop.{nameof(CustomerOrderProduct.Quantity)}) -sum(isNUll(copf.{nameof(CustomerOrderProductFill.QuantityPacked)},0))) > 0  
					)bol on co.{nameof(CustomerOrder.CustomerOrderID)} = bol.CustomerOrderID 
				    JOIN CustomerShippingInfo csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
					JOIN [dbo].[Customer] c on c.CustomerID = co.{nameof(CustomerOrder.CustomerID)}  
					JOIN [dbo].[CustomerOrderProduct] cp on co.{nameof(CustomerOrder.CustomerOrderID)} = cp.{nameof(CustomerOrderProduct.CustomerOrderID)}  and bol.productId = cp.{nameof(CustomerOrderProduct.ProductID)}   
					JOIN [dbo].[Product] p on cp.{nameof(CustomerOrderProduct.ProductID)} = p.{nameof(Product.ProductID)}  

					WHERE co.IsDoNotFill = 0
					AND	co.VPApprovedOn is not NULL
					";

			//AI generated testing then will delete previous code
			//			var queryText = $@"
			//    Select co.{nameof(CustomerOrder.SubmitDate)},
			//           co.{nameof(CustomerOrder.PONumber)},
			//           co.{nameof(CustomerOrder.CustomerOrderCustomID)},
			//           c.{nameof(Customer.CustomerCustomID)},
			//           c.{nameof(Customer.CustomerName)},
			//           p.{nameof(Product.ProductCustomID)}, 
			//           bol.backorder Quantity, 
			//           p.{nameof(Product.ProductName)},
			//           cp.{nameof(CustomerOrderProduct.CustomerOrderID)},
			//           cp.{nameof(CustomerOrderProduct.Price)},
			//           co.{nameof(CustomerOrder.IsFilling)},
			//           co.{nameof(CustomerOrder.FilledBy)},
			//           s.{nameof(CustomerOrderShipment.InvoiceTotal)},
			//           bol.Total
			//    from 
			//        [dbo].[CustomerOrder] co 
			//        join (
			//            select coi.{nameof(CustomerOrder.CustomerOrderID)},
			//                   cop.{nameof(Product.ProductID)},
			//                   (sum(cop.{nameof(CustomerOrderProduct.Quantity)}) - sum(isnull(copf.{nameof(CustomerOrderProductFill.QuantityPacked)}, 0))) backorder,
			//                   sum(cop.{nameof(CustomerOrderProduct.Price)} * cop.{nameof(CustomerOrderProduct.Quantity)}) as Total,
			//                   dateadd(DAY, 0, datediff(day, 0, coi.{nameof(CustomerOrder.SubmitDate)})) day
			//            from [dbo].[CustomerOrderProduct] cop 
			//            left join (
			//                select sum({nameof(CustomerOrderProductFill.QuantityPacked)}) as quantityPacked,
			//                       {nameof(CustomerOrderProductFill.CustomerOrderProductID)} 
			//                from [dbo].[CustomerOrderProductFill] 
			//                group by {nameof(CustomerOrderProductFill.CustomerOrderProductID)}
			//            ) copf on cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)}
			//            left join [dbo].[CustomerOrder] coi on cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = coi.{nameof(CustomerOrder.CustomerOrderID)}
			//            left join [dbo].[CustomerOrderShipment] s on s.{nameof(CustomerOrderShipment.CustomerOrderID)} = coi.{nameof(CustomerOrder.CustomerOrderID)}
			//            where coi.{nameof(CustomerOrder.SubmitDate)} is not null
			//            group by dateadd(DAY, 0, datediff(day, 0, coi.{nameof(CustomerOrder.SubmitDate)})), 
			//                     coi.{nameof(CustomerOrder.CustomerOrderID)}, 
			//                     cop.{nameof(Product.ProductID)}
			//            having (max(cop.{nameof(CustomerOrderProduct.Quantity)}) - sum(isnull(copf.quantityPacked, 0))) > 0  
			//        ) bol on co.{nameof(CustomerOrder.CustomerOrderID)} = bol.{nameof(CustomerOrder.CustomerOrderID)}
			//        join [dbo].[CustomerShippingInfo] csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
			//        join [dbo].[Customer] c on c.{nameof(Customer.CustomerID)} = co.{nameof(CustomerOrder.CustomerID)}
			//        join [dbo].[CustomerOrderProduct] cp on co.{nameof(CustomerOrder.CustomerOrderID)} = cp.{nameof(CustomerOrderProduct.CustomerOrderID)} and bol.{nameof(Product.ProductID)} = cp.{nameof(CustomerOrderProduct.ProductID)}
			//        join [dbo].[Product] p on cp.{nameof(CustomerOrderProduct.ProductID)} = p.{nameof(Product.ProductID)}
			//        join [dbo].[CustomerOrderShipment] s on s.{nameof(CustomerOrderShipment.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
			//    where co.IsDoNotFill = 0
			//    and co.VPApprovedOn is not NULL
			//";




			if (!showAll)
			{
				var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
				if (showInternational)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
				}
				if (showDomesticDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
				}
				if (showDomesticNonDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
				}
				roleFilter += ")";
				queryText += roleFilter;
			}



			if (!string.IsNullOrEmpty(search.Trim()))
			{
				queryText += $@"
					AND (";
				queryText += $@"
					 co.{nameof(CustomerOrder.PONumber)} LIKE :searchTerm
						OR {nameof(CustomerOrder.CustomerOrderCustomID)} LIKE :searchTerm
						OR c.{nameof(Customer.CustomerCustomID)} LIKE :searchTerm
						OR c.{nameof(Customer.CustomerName)} LIKE :searchTerm
						OR p.{nameof(Product.ProductCustomID)} LIKE :searchTerm
						OR cp.{nameof(CustomerOrderProduct.Quantity)} LIKE :searchTerm
						OR p.{nameof(Product.ProductName)}  LIKE :searchTerm ";
				queryText += ") ";
			}

				if(!string.IsNullOrEmpty(searchProductID))
                {
					queryText += $"AND p.{nameof(Product.ProductCustomID)} LIKE '%{searchProductID.Trim()}%' ";
                }

			

			 //queryText += $@"group by co.{nameof(CustomerOrder.SubmitDate)},
				//	co.PONumber,
				//		co.{nameof(CustomerOrder.CustomerOrderCustomID)},
				//		c.{nameof(Customer.CustomerCustomID)},
				//		co.{nameof(CustomerOrder.CustomerOrderID)},
				//		p.{nameof(Product.ProductCustomID)},
				//		c.{nameof(Customer.CustomerName)} ,
				//		bol.backorder , 
				//		p.{nameof(Product.ProductName)}  ,
				//	   cp.{nameof(CustomerOrderProduct.CustomerOrderID)} ";
			
			if(sortCol.ToLower()=="quantity")
            {
				sortCol = "backorder";

			}
			if (sortCol.ToLower() == "customerid")
			{
				sortCol = "customercustomid";

			}
			var sortProp = new List<string>(){
				"ProductCustomID",
				"CustomerOrderID",
				"SubmitDate",
				"CustomerOrderCustomID",
				"CustomerCustomID",
				"backorder",
				"CustomerName",
				"ProductName",
				"PONumber"
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

            if (!sortProp.IsNullOrEmpty())
             queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			
			var query = _dbContext.Session.CreateSQLQuery(queryText);
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				query.SetString("searchTerm", $"%{search}%");
			}

			return query.DynamicListAsync();
		}

		public async Task<IList<dynamic>> GetWaitingOnSubmissionList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			statusFilter = $"co.{nameof(CustomerOrder.SubmitDate)} IS NULL";

			var queryText = CreateOrderListQueryText(additionalColumns, startingTable, status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);
			queryText += CreateSearchQuery(search, hasCustomerShipData);
			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}

				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}

			}
		}

		public async Task<IList<dynamic>> GetWaitingManagerApproval(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			statusFilter = $"co.{nameof(CustomerOrder.SubmitDate)} IS NOT NULL AND co.{nameof(CustomerOrder.MGApprovedOn)} IS NULL";

			string queryText = CreateOrderListQueryText(additionalColumns, startingTable, status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);

			queryText += CreateSearchQuery(search, hasCustomerShipData);

			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}

				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}
			}
		}

		public async Task<IList<dynamic>> GetWaitingVPApproval(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			statusFilter = statusFilter = $@"(
						co.{nameof(CustomerOrder.SubmitDate)} IS NOT NULL
						AND co.{nameof(CustomerOrder.MGApprovedOn)} IS NOT NULL
						AND co.{nameof(CustomerOrder.VPApprovedOn)} IS NULL
					)";

			string queryText = CreateOrderListQueryText(additionalColumns, startingTable, status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);

			queryText += CreateSearchQuery(search, hasCustomerShipData);

			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}

				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}
			}
		}

		public async Task<IList<dynamic>> GetShippingInfo(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			hasCustomerShipData = true;
			startingTable = $@"
						CustomerOrderShipment ship
						JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
					";
			additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, null {nameof(CustomerOrderShipment.InvoiceDate)}
						, null {nameof(CustomerOrderShipment.InvoiceNumber)}
						, null {nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CAST (1 as BIT) HasFilled
						, CAST (0 as BIT) HasShipped
					";

			statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 0";

			string queryText = CreateOrderListQueryText(additionalColumns, startingTable, status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);

			if (!showAll)
			{
				var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
				if (showInternational)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
				}
				if (showDomesticDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
				}
				if (showDomesticNonDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
				}
				roleFilter += ")";
				queryText += roleFilter;
			}

			queryText += CreateSearchQuery(search, hasCustomerShipData);
			
			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}

				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}
			}
		}

		public async Task<IList<dynamic>> GetCustomerOrderFillStatusList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
	int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, null {nameof(CustomerOrderShipment.InvoiceDate)}
						, null {nameof(CustomerOrderShipment.InvoiceNumber)}
						, null {nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CASE WHEN EXISTS (SELECT TOP 1 1 FROM [CustomerOrderShipment]
							WHERE {nameof(CustomerOrderShipment.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)})
							THEN CAST (1 as BIT)
							ELSE CAST (0 as BIT) END as HasFilled
						, CAST(0 as BIT) HasShipped";

			var statusFilter = "1 = 1";

			if (status == CustomerOrderStatusEnum.ToBeFilled || status == CustomerOrderStatusEnum.Filling)
			{
				var isFilling = status == CustomerOrderStatusEnum.Filling;
				statusFilter = $@"(
							co.{nameof(CustomerOrder.VPApprovedOn)} IS NOT NULL
							AND (_box.QuantityPacked IS NULL OR _cop.Quantity <> _box.QuantityPacked)
							AND co.{nameof(CustomerOrder.IsDoNotFill)} = 0
							AND co.{nameof(CustomerOrder.IsFilling)} = {(isFilling ? 1 : 0)}
						)";
			}
			else if (status == CustomerOrderStatusEnum.DoNotFill)
			{
				statusFilter = $" co.{nameof(CustomerOrder.IsDoNotFill)} = 1 ";
			}

			string queryText = CreateOrderListQueryText(additionalColumns, startingTable, status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);

			queryText += CreateSearchQuery(search, hasCustomerShipData);

			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}


				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}
			}
		}

		public async Task<IList<dynamic>> GetsInvoicedOrderList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			hasCustomerShipData = true;
			startingTable = $@"
						CustomerOrderShipment ship
						JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
					";
			additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, ship.{nameof(CustomerOrderShipment.InvoiceDate)}
						, ship.{nameof(CustomerOrderShipment.InvoiceNumber)}
						, ship.{nameof(CustomerOrderShipment.InvoiceTotal)}
						, ship.{nameof(CustomerOrderShipment.MasterTrackingNumber)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CAST (1 as BIT) HasFilled
						, CAST (1 as BIT) HasShipped
					";
			if (status == CustomerOrderStatusEnum.ToBeInvoiced)
				statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 1 AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 0";
			else if (status == CustomerOrderStatusEnum.HasBeenInvoiced)
				statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 1 AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 1";

			var minDate = DateTime.MinValue;
			var maxDate = DateTime.MaxValue;
			var now = DateTime.UtcNow;
			switch (dateOption)
			{
				case DateRangeEnum.Today:
					minDate = now.AddDays(-1);
					break;
				case DateRangeEnum.Yesterday:
					minDate = now.AddDays(-2);
					maxDate = now.AddDays(-1);
					break;
				case DateRangeEnum.Last7Days:
					minDate = now.AddDays(-7).Date;
					break;
				case DateRangeEnum.Last30Days:
					minDate = now.AddDays(-30).Date;
					break;
				case DateRangeEnum.ThisMonth:
					minDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
					break;
				case DateRangeEnum.LastMonth:
					minDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
					maxDate = minDate;
					minDate = minDate.AddMonths(-1);
					break;
				case DateRangeEnum.ThisYear:
					minDate = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					maxDate = minDate.AddYears(1);
					break;
				case DateRangeEnum.LastYear:
					minDate = new DateTime(now.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					maxDate = minDate.AddYears(1);
					break;
			}

			//Has been invoiced
			if (dateOption != DateRangeEnum.None)
			{
				statusFilter += $@"
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} >= @minDate
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} < @maxDate
				";
			}

			var queryText = CreateOrderListQueryText(additionalColumns, startingTable, status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);
			queryText += CreateSearchQuery(search, hasCustomerShipData,true);
			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}

				//set cmd variables for min and max dates
				if (dateOption != DateRangeEnum.None)
				{
					cmd.Parameters.AddWithValue("@minDate", minDate);
					cmd.Parameters.AddWithValue("@maxDate", maxDate);
				}

				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}

			}
		}

		public async Task<IList<dynamic>> GetMyOrdersList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			startingTable = $@"
						CustomerOrder co
						LEFT JOIN CustomerOrderShipment ship ON ship.{nameof(CustomerOrderShipment.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
					";
			additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, {nameof(CustomerOrderShipment.InvoiceDate)}
						, null {nameof(CustomerOrderShipment.InvoiceNumber)}
						, null {nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CAST (0 as BIT) HasFilled
						, CAST (0 as BIT) HasShipped
					";
			// changed the syntax of below statusFilter, need to understand why this makes a differrence
			//statusFilter = $"ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 0 OR ship.{nameof(CustomerOrderShipment.InvoiceSent)} IS NULL";
			statusFilter = $@"((
						ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 0
						OR ship.{nameof(CustomerOrderShipment.InvoiceSent)} IS NULL
						OR COALESCE(_cop.Quantity,0) > COALESCE(_box.QuantityPacked,0))
						AND co.IsDoNotFill = 0
					)";

			
			var queryText = CreateOrderListQueryText(additionalColumns, startingTable,status, statusFilter, currentUserID, showAll, showInternational, showDomesticDistributors, showDomesticNonDistributors);
			queryText += CreateSearchQuery(search, hasCustomerShipData);
			queryText += CreateSortOrderQuery(sortCol, sortAsc);

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
					cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
				}

				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<dynamic> list = new List<dynamic>();
					while (await reader.ReadAsync())
					{
						dynamic customerOrderListItem = MapCustomerOrderObject(reader);
						list.Add(customerOrderListItem);
					}

					return list;
				}

			}
		}

		private string CreateOrderListQueryText(string additionalColumns, string startingTable, CustomerOrderStatusEnum status, string statusFilter, int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var queryText = $@"
				SELECT
					co.{nameof(CustomerOrder.CustomerOrderID)}
					,co.{nameof(CustomerOrder.SubmitDate)}
					,co.{nameof(CustomerOrder.CustomerOrderCustomID)}
					,c.{nameof(Customer.CustomerCustomID)}
					,c.{nameof(Customer.CustomerName)}
					,csi.{nameof(CustomerShippingInfo.Name)} as LocationName
					,csi.{nameof(CustomerShippingInfo.City)} as LocationCity
					,co.{nameof(CustomerOrder.PONumber)}
					,_cop.Subtotal
					,u.{nameof(User.SalesRepID)} + ' ' + u.{nameof(User.FirstName)} + ' ' + u.{nameof(User.LastName)} as SalesRep
					,co.{nameof(CustomerOrder.MGApprovedOn)}
					,co.{nameof(CustomerOrder.VPApprovedOn)}
					,_box.QuantityPacked
					,_cop.Quantity		
					,co.{nameof(CustomerOrder.IsDoNotFill)}
					,co.{nameof(CustomerOrder.AttachmentURI)}
,_cop.ItemIDs
					{additionalColumns}
				FROM {startingTable}
				LEFT JOIN Customer c on c.{nameof(Customer.CustomerID)} = co.{nameof(CustomerOrder.CustomerID)}
				LEFT JOIN CustomerShippingInfo csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
				LEFT JOIN [User] u on u.{nameof(User.UserId)} = csi.{nameof(CustomerShippingInfo.RepUserID)}		
				LEFT JOIN (
					SELECT
						{nameof(CustomerOrderProduct.CustomerOrderID)}
						,SUM({nameof(CustomerOrderProduct.Quantity)}) as Quantity
						,SUM({nameof(CustomerOrderProduct.Quantity)} * {nameof(CustomerOrderProduct.Price)}) as Subtotal
						,STRING_AGG(p.{nameof(Product.ProductName)}, ',') as ItemNames
						,STRING_AGG(p.{nameof(Product.ProductCustomID)}, ',') as ItemNums
						,STRING_AGG(p.{nameof(Product.ProductID)}, ',') as ItemIDs
					FROM CustomerOrderProduct cop
					JOIN Product p on p.{nameof(Product.ProductID)} = cop.{nameof(CustomerOrderProduct.ProductID)}
					GROUP BY CustomerOrderID
				) _cop ON _cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
				LEFT JOIN (
					SELECT box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}, SUM(copf.{nameof(CustomerOrderProductFill.QuantityPacked)}) QuantityPacked
					FROM CustomerOrderProductFill copf
					JOIN CustomerOrderShipmentBox box ON box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
					WHERE box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} IS NOT NULL
					GROUP BY box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}
				) _box ON _box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}				
				WHERE {statusFilter}
			";

            var doCustomerFlagsCheck = status != CustomerOrderStatusEnum.ToBeFilled
						&& status != CustomerOrderStatusEnum.ToBeShipped
                        && status != CustomerOrderStatusEnum.Filling
                        && status != CustomerOrderStatusEnum.DoNotFill;

            if (!showAll && doCustomerFlagsCheck)
            {
                var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
                if (showInternational)
                {
                    roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
                }
                if (showDomesticDistributors)
                {
                    roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
                }
                if (showDomesticNonDistributors)
                {
                    roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
                }
                roleFilter += ")";
                queryText += roleFilter;
            }


            return queryText;
		}

		private string CreateSearchQuery(string search, bool hasCustomerShipData, bool SearchMasterTracking=false)
		{
			string queryText = "";
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				var moreSearchFilters = string.Empty;
				if (hasCustomerShipData)
				{
					moreSearchFilters = $@"
						OR ship.{nameof(CustomerOrderShipment.InvoiceDate)} LIKE @searchTerm
						OR ship.{nameof(CustomerOrderShipment.InvoiceNumber)} LIKE @searchTerm
					";
				}
				queryText += $@"
					AND (
						co.{nameof(CustomerOrder.PONumber)} LIKE @searchTerm
						OR c.{nameof(Customer.CustomerName)} LIKE @searchTerm
						OR c.{nameof(Customer.CustomerCustomID)} LIKE @searchTerm
						OR co.{nameof(CustomerOrder.Contact)} LIKE @searchTerm
						OR csi.{nameof(CustomerShippingInfo.Name)} LIKE @searchTerm
						OR csi.{nameof(CustomerShippingInfo.Address)} LIKE @searchTerm
						OR co.{nameof(CustomerOrder.Notes)} LIKE @searchTerm
						OR _cop.ItemNames LIKE @searchTerm
						OR _cop.ItemNums LIKE @searchTerm
						OR co.{nameof(CustomerOrder.CustomerOrderCustomID)} LIKE @searchTerm
						OR co.{nameof(CustomerOrder.CustomerOrderID)} LIKE @searchTerm";

				if (SearchMasterTracking)
					queryText += $@" OR ship.{nameof(CustomerOrderShipment.MasterTrackingNumber)} LIKE @searchTerm";

				queryText += $@"
							{moreSearchFilters}
					)
				
                ";
            }


            return queryText;
		}

		private string CreateSortOrderQuery(string sortCol, bool sortAsc)
		{
			string queryText = "";
			var sortProp = new List<string>(){
				"CustomerOrderID",
				"SubmitDate",
				"CustomerOrderCustomID",
				"CustomerCustomID",
				"CustomerName",
				"LocationName",
				"LocationCity",
				"PONumber",
				"Subtotal",
				"SalesRep",
				"InvoiceDate",
				"InvoiceNumber",
				"InvoiceTotal",
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

			if (!sortProp.IsNullOrEmpty())
				queryText = $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			return queryText;
		}

		private dynamic MapCustomerOrderObject(System.Data.SqlClient.SqlDataReader reader)
		{
			dynamic customerOrderListItem = new ExpandoObject();

			customerOrderListItem.CustomerCustomID = reader["CustomerCustomID"].ToString();
			customerOrderListItem.CustomerName = reader["CustomerName"].ToString();
			customerOrderListItem.CustomerOrderCustomID = reader["CustomerOrderCustomID"].ToString();
			customerOrderListItem.CustomerOrderID = Convert.ToInt32(reader["CustomerOrderID"]);
			customerOrderListItem.InvoiceDate = reader["InvoiceDate"] == DBNull.Value ? null : (DateTime?)reader["InvoiceDate"];
			customerOrderListItem.InvoiceNumber = reader["InvoiceNumber"].ToString();
			customerOrderListItem.InvoiceTotal = reader["InvoiceTotal"] == DBNull.Value ? null : (decimal?)reader["InvoiceTotal"];
			customerOrderListItem.LocationCity = reader["LocationCity"].ToString();
			customerOrderListItem.LocationName = reader["LocationName"].ToString();
			customerOrderListItem.PONumber = reader["PONumber"].ToString();
			customerOrderListItem.SalesRep = reader["SalesRep"].ToString();
			customerOrderListItem.SubmitDate = reader["SubmitDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["SubmitDate"]).ToShortDateString();
			customerOrderListItem.Subtotal = reader["Subtotal"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Subtotal"]);
			customerOrderListItem.CustomerOrderShipmentID = reader["CustomerOrderShipmentID"] == DBNull.Value ? null : (int?)reader["CustomerOrderShipmentID"];
			customerOrderListItem.HasFilled = reader["HasFilled"] == DBNull.Value ? false : (bool)reader["HasFilled"];
			customerOrderListItem.HasShipped = reader["HasShipped"] == DBNull.Value ? false : (bool)reader["HasShipped"];
			customerOrderListItem.AttachmentURI = reader["AttachmentURI"] == DBNull.Value ? null : reader["AttachmentURI"].ToString();
			customerOrderListItem.MGApprovedOn = reader["MGApprovedOn"] == DBNull.Value ? null : (DateTime?)reader["MGApprovedOn"];
			customerOrderListItem.VPApprovedOn = reader["VPApprovedOn"] == DBNull.Value ? null : (DateTime?)reader["VPApprovedOn"];
			customerOrderListItem.IsDoNotFill = reader["IsDoNotFill"] == DBNull.Value ? false : (bool)reader["IsDoNotFill"];
			customerOrderListItem.Quantity = reader["Quantity"] == DBNull.Value ? null : (int?)reader["Quantity"];
			customerOrderListItem.QuantityPacked = reader["QuantityPacked"] == DBNull.Value ? null : (int?)reader["QuantityPacked"];
			customerOrderListItem.FilledBy = reader["FilledBy"] == DBNull.Value ? -1 : (int?)reader["FilledBy"];
			customerOrderListItem.ItemIDs = reader["ItemIDs"] == DBNull.Value ? null : reader["ItemIDs"].ToString();

			return customerOrderListItem;
		}

		[Obsolete("Splitting into separate calls")]
		public async Task<IList<CustomerOrderListItem>> GetCustomerOrderList(string search, string sortCol, bool sortAsc, CustomerOrderStatusEnum status, DateRangeEnum dateOption,
			int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var hasCustomerShipData = false;
			var startingTable = "CustomerOrder co";
			var additionalColumns = $@"
				, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				, null {nameof(CustomerOrderShipment.InvoiceDate)}
				, null {nameof(CustomerOrderShipment.InvoiceNumber)}
				, null {nameof(CustomerOrderShipment.InvoiceTotal)}
				, CAST (0 as BIT) HasFilled
				, CAST (0 as BIT) HasShipped
				,co.{nameof(CustomerOrder.FilledBy)}
			";

			var statusFilter = "1 = 1";
			switch(status) {
				case CustomerOrderStatusEnum.WaitingSubmission:
					statusFilter = $"co.{nameof(CustomerOrder.SubmitDate)} IS NULL";
					break;
				case CustomerOrderStatusEnum.WaitingManagerApproval:
					statusFilter = $"co.{nameof(CustomerOrder.SubmitDate)} IS NOT NULL AND co.{nameof(CustomerOrder.MGApprovedOn)} IS NULL";
					break;
				case CustomerOrderStatusEnum.WaitingVPApproval:
					statusFilter = $@"(
						co.{nameof(CustomerOrder.SubmitDate)} IS NOT NULL
						AND co.{nameof(CustomerOrder.MGApprovedOn)} IS NOT NULL
						AND co.{nameof(CustomerOrder.VPApprovedOn)} IS NULL
					)";
					break;
				case CustomerOrderStatusEnum.ToBeFilled:
				case CustomerOrderStatusEnum.DoNotFill:
				case CustomerOrderStatusEnum.Filling:
					additionalColumns = $@"
						, null {nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, null {nameof(CustomerOrderShipment.InvoiceDate)}
						, null {nameof(CustomerOrderShipment.InvoiceNumber)}
						, null {nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CASE WHEN EXISTS (SELECT TOP 1 1 FROM [CustomerOrderShipment]
							WHERE {nameof(CustomerOrderShipment.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)})
							THEN CAST (1 as BIT)
							ELSE CAST (0 as BIT) END as HasFilled
						, CAST(0 as BIT) HasShipped";
					if (status == CustomerOrderStatusEnum.ToBeFilled || status == CustomerOrderStatusEnum.Filling)
					{
						var isFilling = status == CustomerOrderStatusEnum.Filling;
						statusFilter = $@"(
							co.{nameof(CustomerOrder.VPApprovedOn)} IS NOT NULL
							AND (_box.QuantityPacked IS NULL OR _cop.Quantity <> _box.QuantityPacked)
							AND co.{nameof(CustomerOrder.IsDoNotFill)} = 0
							AND co.{nameof(CustomerOrder.IsFilling)} = {(isFilling ? 1 : 0)}
						)";
					}
					else if (status == CustomerOrderStatusEnum.DoNotFill)
					{
						statusFilter = $" co.{nameof(CustomerOrder.IsDoNotFill)} = 1 ";
					}
					break;
				case CustomerOrderStatusEnum.ToBeShipped:
					hasCustomerShipData = true;
					startingTable = $@"
						CustomerOrderShipment ship
						JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
					";
					additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, null {nameof(CustomerOrderShipment.InvoiceDate)}
						, null {nameof(CustomerOrderShipment.InvoiceNumber)}
						, null {nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CAST (1 as BIT) HasFilled
						, CAST (0 as BIT) HasShipped
					";

					statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 0";
					break;
				case CustomerOrderStatusEnum.ToBeInvoiced:
				case CustomerOrderStatusEnum.HasBeenInvoiced:
					hasCustomerShipData = true;
					startingTable = $@"
						CustomerOrderShipment ship
						JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
					";
					additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, ship.{nameof(CustomerOrderShipment.InvoiceDate)}
						, ship.{nameof(CustomerOrderShipment.InvoiceNumber)}
						, ship.{nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CAST (1 as BIT) HasFilled
						, CAST (1 as BIT) HasShipped
					";
					if(status == CustomerOrderStatusEnum.ToBeInvoiced)
						statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 1 AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 0";
					else if (status == CustomerOrderStatusEnum.HasBeenInvoiced)
						statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 1 AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 1";

					break;
				case CustomerOrderStatusEnum.ShowMyOrders:
					startingTable = $@"
						CustomerOrder co
						LEFT JOIN CustomerOrderShipment ship ON ship.{nameof(CustomerOrderShipment.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
					";
					additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, null {nameof(CustomerOrderShipment.InvoiceDate)}
						, null {nameof(CustomerOrderShipment.InvoiceNumber)}
						, null {nameof(CustomerOrderShipment.InvoiceTotal)}
						,co.{nameof(CustomerOrder.FilledBy)}
						, CAST (0 as BIT) HasFilled
						, CAST (0 as BIT) HasShipped
					";
					// changed the syntax of below statusFilter, need to understand why this makes a differrence
					//statusFilter = $"ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 0 OR ship.{nameof(CustomerOrderShipment.InvoiceSent)} IS NULL";
					statusFilter = $@"((
						ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 0
						OR ship.{nameof(CustomerOrderShipment.InvoiceSent)} IS NULL
						OR COALESCE(_cop.Quantity,0) > COALESCE(_box.QuantityPacked,0))
						AND co.IsDoNotFill = 0
					)";
					break;
				case CustomerOrderStatusEnum.OnBackOrder:
					break;
			}

			var minDate = DateTime.MinValue;
			var maxDate = DateTime.MaxValue;
			var now = DateTime.UtcNow;
			switch (dateOption) {
				case DateRangeEnum.Today:
					minDate = now.AddDays(-1);
					break;
				case DateRangeEnum.Yesterday:
					minDate = now.AddDays(-2);
					maxDate = now.AddDays(-1);
					break;
				case DateRangeEnum.Last7Days:
					minDate = now.AddDays(-7).Date;
					break;
				case DateRangeEnum.Last30Days:
					minDate = now.AddDays(-30).Date;
					break;
				case DateRangeEnum.ThisMonth:
					minDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
					break;
				case DateRangeEnum.LastMonth:
					minDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
					maxDate = minDate;
					minDate = minDate.AddMonths(-1);
					break;
				case DateRangeEnum.ThisYear:
					minDate = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					maxDate = minDate.AddYears(1);
					break;
				case DateRangeEnum.LastYear:
					minDate = new DateTime(now.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					maxDate = minDate.AddYears(1);
					break;
			}

			//Has been invoiced
			if(dateOption != DateRangeEnum.None) {
				statusFilter += $@"
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} >= @minDate
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} < @maxDate
				";
			}


			var queryText = $@"
				SELECT
					co.{nameof(CustomerOrder.CustomerOrderID)}
					,co.{nameof(CustomerOrder.SubmitDate)}
					,co.{nameof(CustomerOrder.CustomerOrderCustomID)}
					,c.{nameof(Customer.CustomerCustomID)}
					,c.{nameof(Customer.CustomerName)}
					,csi.{nameof(CustomerShippingInfo.Name)} as LocationName
					,csi.{nameof(CustomerShippingInfo.City)} as LocationCity
					,co.{nameof(CustomerOrder.PONumber)}
					,_cop.Subtotal
					,u.{nameof(User.SalesRepID)} + ' ' + u.{nameof(User.FirstName)} + ' ' + u.{nameof(User.LastName)} as SalesRep
					,co.{nameof(CustomerOrder.MGApprovedOn)}
					,co.{nameof(CustomerOrder.VPApprovedOn)}
					,_box.QuantityPacked
					,_cop.Quantity
					,co.{nameof(CustomerOrder.IsDoNotFill)}
					,co.{nameof(CustomerOrder.AttachmentURI)}
					{additionalColumns}
				FROM {startingTable}
				LEFT JOIN Customer c on c.{nameof(Customer.CustomerID)} = co.{nameof(CustomerOrder.CustomerID)}
				LEFT JOIN CustomerShippingInfo csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
				LEFT JOIN [User] u on u.{nameof(User.UserId)} = csi.{nameof(CustomerShippingInfo.RepUserID)}
				LEFT JOIN (
					SELECT
						{nameof(CustomerOrderProduct.CustomerOrderID)}
						,SUM({nameof(CustomerOrderProduct.Quantity)}) as Quantity
						,SUM({nameof(CustomerOrderProduct.Quantity)} * {nameof(CustomerOrderProduct.Price)}) as Subtotal
						,STRING_AGG(p.{nameof(Product.ProductName)}, ',') as ItemNames
						,STRING_AGG(p.{nameof(Product.ProductCustomID)}, ',') as ItemNums
					FROM CustomerOrderProduct cop
					JOIN Product p on p.{nameof(Product.ProductID)} = cop.{nameof(CustomerOrderProduct.ProductID)}
					GROUP BY CustomerOrderID
				) _cop ON _cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
				LEFT JOIN (
					SELECT box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}, SUM(copf.{nameof(CustomerOrderProductFill.QuantityPacked)}) QuantityPacked
					FROM CustomerOrderProductFill copf
					JOIN CustomerOrderShipmentBox box ON box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
					WHERE box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} IS NOT NULL
					GROUP BY box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}
				) _box ON _box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}

				WHERE {statusFilter}
			";

			var doCustomerFlagsCheck = status != CustomerOrderStatusEnum.ToBeFilled
									&& status != CustomerOrderStatusEnum.ToBeShipped
									&& status != CustomerOrderStatusEnum.Filling
									&& status != CustomerOrderStatusEnum.DoNotFill;

			if (!showAll && doCustomerFlagsCheck)
			{
				var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
				if (showInternational)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
				}
				if (showDomesticDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
				}
				if (showDomesticNonDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
				}
				roleFilter += ")";
				queryText += roleFilter;
			}
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				var moreSearchFilters = string.Empty;
				if(hasCustomerShipData){
					moreSearchFilters = $@"
						OR ship.{nameof(CustomerOrderShipment.InvoiceDate)} LIKE @searchTerm
						OR ship.{nameof(CustomerOrderShipment.InvoiceNumber)} LIKE @searchTerm
					";
				}
				queryText += $@"
					AND (
						co.{nameof(CustomerOrder.PONumber)} LIKE @searchTerm
						OR c.{nameof(Customer.CustomerName)} LIKE @searchTerm
						OR c.{nameof(Customer.CustomerCustomID)} LIKE @searchTerm
						OR co.{nameof(CustomerOrder.Contact)} LIKE @searchTerm
						OR csi.{nameof(CustomerShippingInfo.Name)} LIKE @searchTerm
						OR csi.{nameof(CustomerShippingInfo.Address)} LIKE @searchTerm
						OR co.{nameof(CustomerOrder.Notes)} LIKE @searchTerm
						OR _cop.ItemNames LIKE @searchTerm
						OR _cop.ItemNums LIKE @searchTerm
						OR co.{nameof(CustomerOrder.CustomerOrderCustomID)} LIKE @searchTerm
						OR co.{nameof(CustomerOrder.CustomerOrderID)} LIKE @searchTerm
						{moreSearchFilters}
					)
				";
			}

			var sortProp = new List<string>(){
				"CustomerOrderID",
				"SubmitDate",
				"CustomerOrderCustomID",
				"CustomerCustomID",
				"CustomerName",
				"LocationName",
				"LocationCity",
				"PONumber",
				"Subtotal",
				"SalesRep",
				"InvoiceDate",
				"InvoiceNumber",
				"InvoiceTotal",
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

			if (!sortProp.IsNullOrEmpty())
				queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			//needed to use sql connection instead of NHibernate because of the complex query
			//There was a performance issue with NHibernate for waiting on submission and my orders
			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				await client.OpenAsync();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				//set cmd variables for search
				if (!string.IsNullOrEmpty(search.Trim()))
				{
                    cmd.Parameters.AddWithValue("@searchTerm", $"%{search}%");
                }
				//set cmd variables for min and max dates
				if (dateOption != DateRangeEnum.None)
				{
                    cmd.Parameters.AddWithValue("@minDate", minDate);
                    cmd.Parameters.AddWithValue("@maxDate", maxDate);
                }

				
				
				using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{

					List<CustomerOrderListItem> list = new List<CustomerOrderListItem>();
					while (await reader.ReadAsync())
					{
						CustomerOrderListItem customerOrderListItem = new CustomerOrderListItem()
						{
							CustomerCustomID = reader["CustomerCustomID"].ToString(),
							CustomerName = reader["CustomerName"].ToString(),
							CustomerOrderCustomID = reader["CustomerOrderCustomID"].ToString(),
							CustomerOrderID = Convert.ToInt32(reader["CustomerOrderID"]),
							InvoiceDate = reader["InvoiceDate"] == DBNull.Value ? null : (DateTime?)reader["InvoiceDate"],
							InvoiceNumber = reader["InvoiceNumber"].ToString(),
							InvoiceTotal = reader["InvoiceTotal"] == DBNull.Value ? null : (decimal?)reader["InvoiceTotal"],
							LocationCity = reader["LocationCity"].ToString(),
							LocationName = reader["LocationName"].ToString(),
							PONumber = reader["PONumber"].ToString(),
							SalesRep = reader["SalesRep"].ToString(),
							SubmitDate = reader["SubmitDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["SubmitDate"]).ToShortDateString(),
							Subtotal = reader["Subtotal"] == DBNull.Value ? null : Convert.ToDecimal(reader["Subtotal"]),
							CustomerOrderShipmentID = reader["CustomerOrderShipmentID"] == DBNull.Value ? null : (int?)reader["CustomerOrderShipmentID"],
							HasFilled = reader["HasFilled"] == DBNull.Value ? false : (bool)reader["HasFilled"],
							HasShipped = reader["HasShipped"] == DBNull.Value ? false : (bool)reader["HasShipped"],
							AttachmentURI = reader["AttachmentURI"] == DBNull.Value ? null : reader["AttachmentURI"].ToString(),
							MGApprovedOn = reader["MGApprovedOn"] == DBNull.Value ? null : (DateTime?)reader["MGApprovedOn"],
							VPApprovedOn = reader["VPApprovedOn"] == DBNull.Value ? null : (DateTime?)reader["VPApprovedOn"],
							IsDoNotFill = reader["IsDoNotFill"] == DBNull.Value ? false : (bool)reader["IsDoNotFill"],
							Quantity = reader["Quantity"] == DBNull.Value ? null : (int?)reader["Quantity"],
							QuantityPacked = reader["QuantityPacked"] == DBNull.Value ? null : (int?)reader["QuantityPacked"],
							FilledBy = reader["FilledBy"] == DBNull.Value ? -1 : (int?)reader["FilledBy"],
						};

						list.Add(customerOrderListItem);
					}


					return list;// query.DynamicListAsync();
				}
			}

                

                //var query = _dbContext.Session.CreateSQLQuery(queryText);
                //if (!string.IsNullOrEmpty(search.Trim()))
                //{
                //	query.SetString("searchTerm", $"%{search}%");
                //}
                //if(dateOption != DateRangeEnum.None) {
                //	query.SetDateTime("minDate", minDate);
                //	query.SetDateTime("maxDate", maxDate);
                //}

                //return query.DynamicListAsync();
            }
		public CustomerOrder GetCustomerOrder(int customerOrderID)
		{
			var customerOrder = _dbContext.Get<CustomerOrder>(customerOrderID);
			return customerOrder;
		}

		public CustomerOrder GetCustomerOrder(int customerOrderID, int currentUserID, bool showAll, bool showInternational,
			bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var customerOrder = _dbContext.Get<CustomerOrder>(customerOrderID);
			if (!showAll)
			{
				var customer = _dbContext.Get<Customer>(customerOrder.CustomerID);
				var authorized = false;
				if (showInternational && customer.IsInternational)
				{
					authorized = true;
				}
				if (showDomesticDistributors && customer.IsDomesticDistributor)
				{
					authorized = true;
				}
				if (showDomesticNonDistributors && (customer.IsDomestic || customer.IsDomesticAfaxys))
				{
					authorized = true;
				}
				if (!authorized)
				{
					var shippingInfo = _dbContext.Get<CustomerShippingInfo>(customerOrder.CustomerShippingInfoID);
					if(shippingInfo.RepUserID != currentUserID)
						customerOrder = null;
				}
			}
			return customerOrder;
		}

		public CustomerOrder GetCustomerOrderByPONumber(string poNumber)
		{
			var queryText = $@"
				SELECT * FROM CustomerOrder
				WHERE {nameof(CustomerOrder.PONumber)} = :poNumber
			";
			var customerOrder = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetString("poNumber", poNumber)
				.DynamicList().FirstOrDefault();

			if (customerOrder == null)
				return new CustomerOrder();
			
			return new CustomerOrder(customerOrder);
		}

		public CustomerOrder GetCustomerOrderByCONumber(string customCustomerOrderNumber)
		{
			var queryText = $@"
				SELECT * FROM CustomerOrder
				WHERE {nameof(CustomerOrder.CustomerOrderCustomID)} = :coNumber
			";
			var customerOrder = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetString("coNumber", customCustomerOrderNumber)
				.DynamicList().FirstOrDefault();

			if (customerOrder == null)
				return new CustomerOrder();

			return new CustomerOrder(customerOrder);
		}

		public List<CustomerOrder> GetCustomerOrderByCustomerId(int customerId)
		{
			List<CustomerOrder> customerOrders = new List<CustomerOrder>();
			var queryText = $@"
				SELECT * FROM CustomerOrder
				WHERE {nameof(CustomerOrder.CustomerID)} = :customerId AND {nameof(CustomerOrder.IsDoNotFill)} = 0 AND {nameof(CustomerOrder.ShippedByOn)} IS NULL
			";
			var customerOrder = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("customerId", customerId)
				.DynamicList();

			foreach(var co in customerOrder)
			{
				customerOrders.Add(new CustomerOrder(co));
			}

			return customerOrders;
		}

		public CustomerOrderFillInfo GetCustomerOrderFillInfo(int customerOrderID)
		{
			var queryText = $@"
				SELECT
					co.{nameof(CustomerOrder.CustomerOrderID)}
					,co.{nameof(CustomerOrder.CustomerOrderCustomID)}
					,co.{nameof(CustomerOrder.SubmitDate)}
					,co.{nameof(CustomerOrder.CustomerShippingInfoID)}
					,co.{nameof(CustomerOrder.IntermediaryShippingName)}
					,co.{nameof(CustomerOrder.IntermediaryShippingAddress)}
					,co.{nameof(CustomerOrder.IntermediaryShippingContactName)}
					,co.{nameof(CustomerOrder.IntermediaryShippingContactNumber)}
					,co.{nameof(CustomerOrder.IntermediaryShippingContactEmail)}
					,shipCompany.{nameof(Code.CodeDescription)} ShipCompany
					,shipMethod.{nameof(Code.CodeDescription)} ShipMethod
					,co.{nameof(CustomerOrder.ShipCompanyType)}
					,csi.{nameof(CustomerShippingInfo.ShipCompany1CodeID)}
					,csi.{nameof(CustomerShippingInfo.ShipCompany2CodeID)}
					,csi.{nameof(CustomerShippingInfo.ShipCompany1AccountNumber)}
					,csi.{nameof(CustomerShippingInfo.ShipCompany2AccountNumber)}
					,co.{nameof(CustomerOrder.IsPartialShipAcceptable)}
					,u.{nameof(User.UserId)} SalesRepID
					,c.{nameof(Customer.CustomerID)}
					,c.{nameof(Customer.CustomerCustomID)}
					,c.{nameof(Customer.CustomerName)}
					,c.{nameof(Customer.CountryCodeID)}
					,c.{nameof(Customer.IsInternational)} IsInternationalCustomer
					,csi.{nameof(CustomerShippingInfo.Name)} CustomerShipName
					,csi.{nameof(CustomerShippingInfo.Address)} CustomerShipAddress
					,csi.{nameof(CustomerShippingInfo.Address2)} CustomerShipAddress2
					,csi.{nameof(CustomerShippingInfo.City)} CustomerShipCity
					,csiState.{nameof(Code.CodeDescription)} CustomerShipState
					,csi.{nameof(CustomerShippingInfo.ZipCode)} CustomerShipZip
					,csiCountry.{nameof(Code.CodeDescription)} CustomerShipCountry
					,co.{nameof(CustomerOrder.Notes)}
					,co.{nameof(CustomerOrder.Instructions)}
					,co.{nameof(CustomerOrder.UpdatedBy)}
					,co.{nameof(CustomerOrder.UpdatedOn)}
					,co.{nameof(CustomerOrder.PONumber)}
					,co.{nameof(CustomerOrder.IsFilling)}
					,co.{nameof(CustomerOrder.FilledByOn)}
					,co.{nameof(CustomerOrder.FilledBy)}
					,co.{nameof(CustomerOrder.FinancingApproved)}
					,co.{nameof(CustomerOrder.FinancingApprovedBy)}
					,co.{nameof(CustomerOrder.FinancingApprovedOn)}

				FROM CustomerOrder co
				LEFT JOIN Customer c ON c.{nameof(Customer.CustomerID)} = co.{nameof(Customer.CustomerID)}
				LEFT JOIN CustomerShippingInfo csi ON csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
				LEFT JOIN [User] u ON u.{nameof(User.UserId)} = csi.{nameof(CustomerShippingInfo.RepUserID)}
				LEFT JOIN Code shipCompany ON shipCompany.{nameof(Code.CodeID)} = co.{nameof(CustomerOrder.ShipCompanyType)}
				LEFT JOIN Code shipMethod ON shipMethod.{nameof(Code.CodeID)} = co.{nameof(CustomerOrder.ShipChoiceCodeID)}
				LEFT JOIN Code csiState ON csiState.{nameof(Code.CodeID)} = csi.{nameof(CustomerShippingInfo.StateCodeID)}
				LEFT JOIN Code csiCountry ON csiCountry.{nameof(Code.CodeID)} = csi.{nameof(CustomerShippingInfo.CountryCodeID)}
				WHERE co.{nameof(CustomerOrder.CustomerOrderID)} = :customerOrderID
			";

			var cofi = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("customerOrderID", customerOrderID)
				.Dynamic();

			return new CustomerOrderFillInfo(cofi);
		}

		public IList<dynamic> GetCustomerOrderFillProducts(int customerOrderID, int? boxID)
		{
			var queryText = $@"
				SELECT
					copf.{nameof(CustomerOrderProductFill.CustomerOrderProductFillID)}
					,cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
					,p.{nameof(Product.ProductID)}
					,p.{nameof(Product.ProductCustomID)}
					,p.{nameof(Product.ProductName)}
					,Code.{nameof(Code.CodeDescription)} UnitOfMeasure
					,cop.{nameof(CustomerOrderProduct.Quantity)} OrderQuantity
					,_qtyPacked.{nameof(CustomerOrderProductFill.QuantityPacked)} QuantityAlreadyPacked
					,copf.{nameof(CustomerOrderProductFill.QuantityPacked)}
					,copf.{nameof(CustomerOrderProductFill.SerialNumbers)}
				FROM CustomerOrderProduct cop
				LEFT JOIN Product p ON p.{nameof(Product.ProductID)} = cop.{nameof(CustomerOrderProduct.ProductID)}
				LEFT JOIN Code ON Code.{nameof(Code.CodeID)} = p.{nameof(Product.UnitOfMeasureCodeID)}
				LEFT JOIN (
					SELECT
						{nameof(CustomerOrderProductFill.CustomerOrderProductID)}
						,SUM({nameof(CustomerOrderProductFill.QuantityPacked)}) {nameof(CustomerOrderProductFill.QuantityPacked)}
					FROM CustomerOrderProductFill copf
					GROUP BY CustomerOrderProductID
				) _qtyPacked ON _qtyPacked.{nameof(CustomerOrderProductFill.CustomerOrderProductID)} = cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
				LEFT JOIN CustomerOrderProductFill copf ON
						copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)} = cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
						AND copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)} = :boxID
				WHERE cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = :customerOrderID
			";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("customerOrderID", customerOrderID)
				.SetInt32("boxID", boxID ?? -1)
				.DynamicList();
		}

        public IList<dynamic> GetAllFillProducts(string search, string sortCol, bool sortAsc, DateTime startTime, DateTime EndTime)
        {
            var queryText = $@"
				SELECT
					copf.{nameof(CustomerOrderProductFill.CustomerOrderProductFillID)}
					,cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
					,p.{nameof(Product.ProductID)}
					,p.{nameof(Product.ProductCustomID)}
					,p.{nameof(Product.ProductName)}
					,p.{nameof(Product.PriceInternationalDistribution)}
					,p.{nameof(Product.PriceDomesticAfaxys)}
					,p.{nameof(Product.PriceDomesticList)}
					,cop.{nameof(CustomerOrderProduct.Quantity)} OrderQuantity
					,cos.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
					,cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)}
					,cop.{nameof(CustomerOrderProduct.Price)}
					,cos.{nameof(CustomerOrderShipment.InvoiceTotal)}
					,c.{nameof(Customer.CustomerCustomID)}
					,copf.{nameof(CustomerOrderProductFill.QuantityPacked)}
					,copf.{nameof(CustomerOrderProductFill.SerialNumbers)}
					,cos.{nameof(CustomerOrderShipment.InvoiceNumber)}
					,cos.{nameof(CustomerOrderShipment.InvoiceDate)}
				FROM CustomerOrderProduct cop
				LEFT JOIN Product p ON p.{nameof(Product.ProductID)} = cop.{nameof(CustomerOrderProduct.ProductID)}
				LEFT JOIN CustomerOrderShipment cos ON cos.{nameof(CustomerOrderShipment.CustomerOrderID)} = cop.{nameof(CustomerOrderProduct.CustomerOrderID)}
				LEFT JOIN CustomerOrderShipmentBox cosb ON cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} = cos.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
				LEFT JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = cos.{nameof(CustomerOrderShipment.CustomerOrderID)}
				LEFT JOIN CustomerOrderProductFill copf ON copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)} = cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
				LEFT JOIN Customer c ON c.{nameof(Customer.CustomerID)} = co.{nameof(CustomerOrder.CustomerID)}	
				WHERE cos.InvoiceDate>=:startDate AND cos.InvoiceDate<=:endDate AND ((cos.{nameof(CustomerOrderShipment.ShipmentComplete)} = :shipmentComplete) OR (cos.{nameof(CustomerOrderShipment.ShipmentComplete)} != :shipmentComplete AND copf.{nameof(CustomerOrderProductFill.CustomerOrderProductFillID)} IS NOT NULL AND copf.{nameof(CustomerOrderProductFill.CustomerOrderProductFillID)} > 0 AND co.{nameof(CustomerOrder.IsDoNotFill)} != :doNotFill)) 
			";

            if (!string.IsNullOrEmpty(search.Trim()))
            {
                queryText += $@"
					AND (";
				queryText += $@"
					 copf.{nameof(CustomerOrderProductFill.CustomerOrderProductFillID)} LIKE :searchTerm
					OR cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)} LIKE :searchTerm
					OR p.{nameof(Product.ProductID)} LIKE :searchTerm
					OR p.{nameof(Product.ProductCustomID)} LIKE :searchTerm
					OR p.{nameof(Product.ProductName)} LIKE :searchTerm
					OR p.{nameof(Product.PriceInternationalDistribution)} LIKE :searchTerm
					OR p.{nameof(Product.PriceDomesticAfaxys)} LIKE :searchTerm
					OR p.{nameof(Product.PriceDomesticList)} LIKE :searchTerm
					OR cop.{nameof(CustomerOrderProduct.Quantity)} LIKE :searchTerm
					OR cos.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)} LIKE :searchTerm
					OR cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} LIKE :searchTerm
					OR cop.{nameof(CustomerOrderProduct.Price)} LIKE :searchTerm
					OR cos.{nameof(CustomerOrderShipment.InvoiceTotal)}  LIKE :searchTerm
					OR copf.{nameof(CustomerOrderProductFill.QuantityPacked)} LIKE :searchTerm
					OR copf.{nameof(CustomerOrderProductFill.SerialNumbers)} LIKE :searchTerm
					OR cos.{nameof(CustomerOrderShipment.InvoiceNumber)} LIKE :searchTerm
					OR c.{nameof(Customer.CustomerCustomID)} LIKE :searchTerm ";
                queryText += ") ";
            }

            var sortProp = new List<string>(){
                "CustomerOrderProductFillID",
                "CustomerOrderProductID",
                "ProductCustomID",
                "ProductName",
                "PriceInternationalDistribution",
                "PriceDomesticList",
                "PriceDomesticAfaxys",
                "PriceInternationalDistribution",
                "QuantityPacked",
                "CustomerOrderShipmentBoxID",
                "CustomerOrderShipmentID",
                "ProductID",
                "InvoiceNumber",
                "Price",
                "InvoiceTotal",
                "CustomerCustomID",
                "SerialNumbers"
            }.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

            if (!sortProp.IsNullOrEmpty())
                queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			var query = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetBoolean("doNotFill", true)
				.SetBoolean("shipmentComplete", true)
				.SetString("startDate", startTime.ToShortDateString())
				.SetString("endDate", EndTime.ToShortDateString());

            if (!string.IsNullOrEmpty(search.Trim()))
            {
                query.SetString("searchTerm", $"%{search}%");
            }

                return query.DynamicList();
        }

        public IEnumerable<dynamic> GetCustomerOrderProducts(int customerOrderID)
		{
			var query = $@"
				SELECT
					cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
					,cop.{nameof(CustomerOrderProduct.CustomerOrderID)}
					,cop.{nameof(CustomerOrderProduct.ProductID)}
					,p.{nameof(Product.ProductName)}
					,p.{nameof(Product.ProductCustomID)}
					,cop.{nameof(CustomerOrderProduct.UnitOfMeasureCodeID)}
					,cop.{nameof(CustomerOrderProduct.Quantity)}
					,cop.{nameof(CustomerOrderProduct.Price)}
				FROM CustomerOrderProduct cop
				JOIN Product p on p.ProductID = cop.ProductID
				WHERE cop.CustomerOrderID = :customerOrderID
			";

			return _dbContext.Session
				.CreateSQLQuery(query)
				.SetInt32("customerOrderID", customerOrderID)
				.DynamicList();
		}

		public Task<IList<dynamic>> GetHistoryForProduct(int customerID, int productID)
		{
			var query = @"
				SELECT TOP 5
					co.SubmitDate
					,cop.Quantity
					,cop.UnitOfMeasureCodeID
					,cop.Price
				FROM CustomerOrderProduct cop
				JOIN CustomerOrder co on co.CustomerOrderID = cop.CustomerOrderID
				WHERE
					ProductID = :productID
					AND co.CustomerID = :customerID
					AND co.SubmitDate IS NOT NULL
				ORDER BY SubmitDate DESC
			";

			return _dbContext.Session
				.CreateSQLQuery(query)
				.SetInt32("productID", productID)
				.SetInt32("customerID", customerID)
				.DynamicListAsync();
		}

		public CustomerOrder SaveCustomerOrder(CustomerOrder customerOrder)
		{
			_dbContext.BeginTransaction();
			_dbContext.Save(customerOrder);
			_dbContext.Commit();

			return customerOrder;
		}

		public bool UpdateCustomerOrder(CustomerOrder customerOrder)
		{
			_dbContext.BeginTransaction();
			_dbContext.SaveOrUpdate(customerOrder);
			_dbContext.Commit();
			return true;
		}

		public CustomerOrder GetCustomerOrderByPONumber(int customerId, string poNumber)
		{
			var query = $@"
				SELECT TOP 1 co.{nameof(CustomerOrder.CustomerID)}
					,co.{nameof(CustomerOrder.PONumber)}
					,co.{nameof(CustomerOrder.CreatedOn)},
                    co.{nameof(CustomerOrder.CustomerOrderID)},
                    co.{nameof(CustomerOrder.CustomerOrderCustomID)}
				FROM CustomerOrder co
				WHERE co.CustomerID = :customerID AND co.PONumber = :poNumber
				ORDER BY co.CreatedOn ASC
			";

			IList<dynamic> results = _dbContext.Session
				.CreateSQLQuery(query)
				.SetInt32("customerID", customerId)
				.SetString("poNumber", poNumber)
				.DynamicList();

			if(results.Count > 0)
			{
				return new CustomerOrder()
				{
					CustomerID = results[0].CustomerID,
					PONumber = results[0].PONumber,
					CreatedOn = results[0].CreatedOn,
					CustomerOrderID = results[0].CustomerOrderID,
					CustomerOrderCustomID = results[0].CustomerOrderCustomID
					
				};
			}

			return default;
		}

		public DailyCustomerOrderCount GetDailyCustomerOrderCount(int customerID)
		{
			return _dbContext.DailyCustomerOrderCounts.FirstOrDefault(x => x.CustomerID == customerID);
		}

		public DailyCustomerOrderCount SaveDailyCustomerOrderCount(int customerID, DateTime now, int count)
		{
			var model = new DailyCustomerOrderCount { CustomerID = customerID, LastCreated = now, DailyCount = count };
			_dbContext.BeginTransaction();
			_dbContext.Save(model);
			_dbContext.Commit();

			return model;
		}

		public void UpdateDailyCustomerOrderCount(DailyCustomerOrderCount count)
		{
			_dbContext.BeginTransaction();
			_dbContext.Update(count);
			_dbContext.Commit();
		}

		public bool SaveCustomerOrderProducts(List<CustomerOrderProduct> customerOrderProducts)
		{
			if(!customerOrderProducts.Any()) {
				return false;
			}

			var properties = typeof(CustomerOrderProduct).GetProperties().Select(p => p.Name);
			var tempTableName = $"TempCustomerOrderProduct{Guid.NewGuid().ToString().Replace("-", "")}";
			var queryText = $@"
				DROP TABLE IF EXISTS #{tempTableName}
				SELECT * INTO #{tempTableName} FROM CustomerOrderProduct where 1 = 0
				SET IDENTITY_INSERT #{tempTableName} ON
				INSERT INTO #{tempTableName} ({string.Join(',', properties)}) VALUES
			";

			for(var i = 0; i < customerOrderProducts.Count; i++) {
				if (i > 0) { queryText += ","; }

				var props = properties.Select(x => $":{x}{i}");
				queryText += $"({string.Join(',', props)})";
			}

			queryText += $@"
				MERGE CustomerOrderProduct as t
					USING(SELECT * FROM #{tempTableName}) as s
						ON s.CustomerOrderProductID = t.CustomerOrderProductID
					WHEN MATCHED THEN
						UPDATE SET
							  ProductID           = s.ProductID
							, UnitOfMeasureCodeID = s.UnitOfMeasureCodeID
							, Quantity            = s.Quantity
							, Price               = s.Price
					WHEN NOT MATCHED BY TARGET THEN
						INSERT VALUES (
							 s.CustomerOrderID
							,s.ProductID
							,s.UnitOfMeasureCodeID
							,s.Quantity
							,s.Price
						)
				;
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			for(var i = 0; i < customerOrderProducts.Count; i++) {
				query.SetInt32($"CustomerOrderProductID{i}", customerOrderProducts[i].CustomerOrderProductID);
				query.SetInt32($"CustomerOrderID{i}", customerOrderProducts[i].CustomerOrderID);
				query.SetInt32($"ProductID{i}", customerOrderProducts[i].ProductID);
				query.SetInt32($"UnitOfMeasureCodeID{i}", customerOrderProducts[i].UnitOfMeasureCodeID);
				query.SetInt32($"Quantity{i}", customerOrderProducts[i].Quantity);
				query.SetDecimal($"Price{i}", customerOrderProducts[i].Price);
			}

			_dbContext.BeginTransaction();
			query.ExecuteUpdate();
			_dbContext.Commit();
			return  true;
		}

		public int RescindCustomerOrderToBeFilled(int customerOrderID)
		{
			var queryText = $@"
				DECLARE @deletedIds TABLE(id int)

				DELETE FROM [CustomerOrderShipmentBox]
				OUTPUT deleted.CustomerOrderShipmentBoxID INTO @deletedIds
				WHERE {nameof(CustomerOrderShipmentBox.CustomerOrderID)} = :customerOrderId

				DELETE FROM  f
				FROM [CustomerOrderProductFill] f
				INNER JOIN @deletedIds as d
				ON f.CustomerOrderShipmentBoxID = d.id

				UPDATE [CustomerOrder]
				SET {nameof(CustomerOrder.VPApprovedBy)} = null, {nameof(CustomerOrder.VPApprovedOn)} = null
				WHERE {nameof(CustomerOrder.CustomerOrderID)} = :customerOrderId
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32($"customerOrderId", customerOrderID);

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public int DeleteCustomerOrderShipment(int shipmentID)
		{
			var queryText = $@"
				DECLARE @deletedIds TABLE(id int)
				DELETE FROM [CustomerOrderShipment]
				WHERE {nameof(CustomerOrderShipment.CustomerOrderShipmentID)} = :shipmentID

				DELETE FROM [CustomerOrderShipmentBox]
				OUTPUT deleted.CustomerOrderShipmentBoxID INTO @deletedIds
				WHERE {nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} = :shipmentID

				DELETE FROM  f
				FROM [CustomerOrderProductFill] f
				INNER JOIN @deletedIds as d
				ON f.CustomerOrderShipmentBoxID = d.id
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32($"shipmentID", shipmentID);

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public bool DeleteCustomerOrder(int customerOrderID)
		{
			var queryText = $@"
			DELETE FROM CustomerOrder 
			WHERE
				CustomerOrderID = :customerOrderId
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);

			query.SetInt32($"customerOrderId", customerOrderID);

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();

			return results > 0;
		}

		public int DeleteCustomerOrderProduct(List<int> customerOrderProductIDs)
		{
			var idStrings = customerOrderProductIDs.Select(x => $":copID{x}");
			var queryText = $@"
			DELETE cop FROM CustomerOrderProduct cop
			JOIN CustomerOrder co ON co.CustomerOrderID = cop.CustomerOrderID
			WHERE
				cop.CustomerOrderProductID IN ({string.Join(',', idStrings)})
				AND co.MGApprovedOn IS NULL
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);

			foreach(var id in customerOrderProductIDs)
				query.SetInt32($"copID{id}", id);

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public int ApproveCustomerOrder(int customerOrderID, int userID, bool isVPApproval)
		{
			var queryText = string.Empty;
			if (isVPApproval)
			{
				queryText = @"
					UPDATE CustomerOrder SET
						VPApprovedOn = GETUTCDATE(),
						VPApprovedBy = :userID
					WHERE
						CustomerOrderID = :customerOrderID
						AND VPApprovedOn IS NULL
				";
			}
			else
			{
				queryText = @"
					UPDATE CustomerOrder SET
						MGApprovedOn = GETUTCDATE(),
						MGApprovedBy = :userID
					WHERE
						CustomerOrderID = :customerOrderID
						AND MGApprovedOn IS NULL
				";
			}

			_dbContext.BeginTransaction();
			var results = _dbContext.Session.CreateSQLQuery(queryText)
				.SetInt32("userID", userID)
				.SetInt32("customerOrderID", customerOrderID)
				.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public int ApproveCustomerOrderFinancing(int customerOrderID, int userID)
        {
			var queryText = $@"
					UPDATE CustomerOrder SET {nameof(CustomerOrder.FinancingApproved)} = :financingApproved,
										 {nameof(CustomerOrder.FinancingApprovedBy)} = :financingApprovedBy,
										 {nameof(CustomerOrder.FinancingApprovedOn)} = :financingApprovedOn
				WHERE {nameof(CustomerOrder.CustomerOrderID)} = :customerOrderID";

			_dbContext.BeginTransaction();
			var results = _dbContext.Session.CreateSQLQuery(queryText)
				.SetInt32("financingApproved", 1)
				.SetInt32("financingApprovedBy", userID)
				.SetDateTime("financingApprovedOn", DateTime.UtcNow)
				.SetInt32("customerOrderID", customerOrderID)
				.ExecuteUpdate();
			_dbContext.Commit();

			return results;

		}

		public CustomerOrderShipment SaveCustomerOrderShipment(CustomerOrderShipment model)
		{
			_dbContext.BeginTransaction();
			_dbContext.Save(model);
			_dbContext.Commit();

			return model;
		}

		public CustomerOrderShipmentBox SaveCustomerOrderShipmentBox(CustomerOrderShipmentBox model)
		{
			_dbContext.BeginTransaction();
			_dbContext.Save(model);
			_dbContext.Commit();

			return model;
		}

		//Remove CustomerOrderShipmentBox
		public void RemoveCustomerOrderShipmentBox(CustomerOrderShipmentBox model)
		{
			_dbContext.BeginTransaction();
			_dbContext.Delete(model);
			_dbContext.Commit();		
		}

		public int MoveFillsIntoShipmentBoxes(int customerOrderID, IEnumerable<CustomerOrderProductFill> models)
		{
			var queryText = $@"
				INSERT INTO CustomerOrderProductFill VALUES
			";

			var properties = new List<string>(){
				nameof(CustomerOrderProductFill.CustomerOrderProductID),
				nameof(CustomerOrderProductFill.QuantityPacked),
				nameof(CustomerOrderProductFill.SerialNumbers),
				nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID),
			};

			var modelList = models.ToList();
			for (var i = 0; i < modelList.Count; i++)
			{
				var model = modelList[i];
				if (i > 0) { queryText += ","; }

				var props = properties.Select(x => $":{x}{i}");
				queryText += $"({string.Join(',', props)})";
			}

			var query = _dbContext.Session.CreateSQLQuery(queryText);

			for (var i = 0; i < modelList.Count; i++)
			{
				var model = modelList[i];
				query.SetInt32($"CustomerOrderProductID{i}", model.CustomerOrderProductID);
				query.SetInt32($"QuantityPacked{i}", model.QuantityPacked);
				query.SetString($"SerialNumbers{i}", model.SerialNumbers);
				query.SetParameter<int?>($"CustomerOrderShipmentBoxID{i}", model.CustomerOrderShipmentBoxID);
			}

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public List<int> GetBoxesForFill(int customerOrderID)
		{
			return (
				from box in _dbContext.CustomerOrderShipmentBoxes
				where
					box.CustomerOrderShipmentID == null
					&& box.CustomerOrderID == customerOrderID
				select box.CustomerOrderShipmentBoxID
			).Distinct().ToList();
		}

		public IEnumerable<CustomerOrderShipmentBox> GetBoxesForShip(int customerOrderShipmentID)
		{
			return _dbContext.CustomerOrderShipmentBoxes.Where(x => x.CustomerOrderShipmentID == customerOrderShipmentID);
		}

		public CustomerOrderShipment GetCustomerOrderShipment(int shipmentID)
		{
			return _dbContext.Get<CustomerOrderShipment>(shipmentID);
		}

		public List<CustomerOrderShipment> GetCustomerOrderShipments(int customerOrderID)
		{
			return _dbContext.CustomerOrderShipments.Where(s => (s.CustomerOrderID == customerOrderID)).OrderByDescending(x => x.InvoiceNumber).ToList();
		}

		public CustomerOrderShipmentBox GetCustomerOrderShipmentBox(int boxID)
		{
			return _dbContext.Get<CustomerOrderShipmentBox>(boxID);
		}

		public IEnumerable<CustomerOrderProductFill> GetShipmentBoxFillInfo(int shipmentBoxId)
		{
			return _dbContext.CustomerOrderProductFills.Where(f => f.CustomerOrderShipmentBoxID == shipmentBoxId);
		}

		public int MoveBoxesIntoShipment(int customerOrderID, int customerOrderShipmentID)
		{
			var queryText = $@"
				UPDATE CustomerOrderShipmentBox SET {nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} = :customerOrderShipmentID
				WHERE {nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} IS NULL
					AND {nameof(CustomerOrderShipmentBox.CustomerOrderID)} = :customerOrderID
			";

			_dbContext.BeginTransaction();
			var result = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("customerOrderID", customerOrderID)
				.SetInt32("customerOrderShipmentID", customerOrderShipmentID)
				.ExecuteUpdate();
			_dbContext.Commit();
			return result;
		}

		public int UpdateFillBoxes(int customerOrderID, int boxID, IEnumerable<CustomerOrderProductFill> models)
		{
			var queryText = string.Empty;

			var modelList = models.ToList();
			for(var i = 0; i < modelList.Count; i++)
			{
				queryText += $@"
					UPDATE CustomerOrderProductFill SET
						{nameof(CustomerOrderProductFill.QuantityPacked)} = :quantityPacked{i}
						,{nameof(CustomerOrderProductFill.SerialNumbers)} = :serialNumbers{i}
					WHERE {nameof(CustomerOrderProductFill.CustomerOrderProductFillID)} = :customerOrderProductFillID{i}
				";
			}

			queryText += $@"
				DELETE FROM CustomerOrderProductFill
					WHERE {nameof(CustomerOrderProductFill.QuantityPacked)} = 0 AND
					{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)} = :boxID
			";

			queryText += $@"
				DELETE FROM CustomerOrderShipmentBox
					WHERE {nameof(CustomerOrderShipmentBox.CustomerOrderID)} = :customerOrderID
					AND {nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)} = :boxID
					AND {nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)}
						NOT IN (
							SELECT {nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
							FROM CustomerOrderProductFill
							WHERE {nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)} = :boxID
						)
			";

			var query = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("customerOrderID", customerOrderID)
				.SetInt32("boxID", boxID);

			for(var i = 0; i < modelList.Count; i++)
			{
				query.SetInt32($"quantityPacked{i}", modelList[i].QuantityPacked);
				query.SetString($"serialNumbers{i}", modelList[i].SerialNumbers);
				query.SetInt32($"customerOrderProductFillID{i}", modelList[i].CustomerOrderProductFillID);
			}

			_dbContext.BeginTransaction();
			var result = query.ExecuteUpdate();
			_dbContext.Commit();

			return result;
		}

		public int UpdateBoxDims(int customerOrderID, int boxID, CustomerOrderShipmentBox box)
		{
			var quertyText = $@"
				UPDATE CustomerOrderShipmentBox SET
					 {nameof(CustomerOrderShipmentBox.Weight)}              = :Weight
					,{nameof(CustomerOrderShipmentBox.WeightUnitCodeID)}    = :WeightUnitCodeID
					,{nameof(CustomerOrderShipmentBox.Length)}              = :Length
					,{nameof(CustomerOrderShipmentBox.Width)}               = :Width
					,{nameof(CustomerOrderShipmentBox.Depth)}               = :Depth
					,{nameof(CustomerOrderShipmentBox.DimensionUnitCodeID)} = :DimensionUnitCodeID
				WHERE {nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = :boxID
			";

			_dbContext.BeginTransaction();
			var results = _dbContext.Session.CreateSQLQuery(quertyText)
				.SetParameter<int?>("Weight", box.Weight)
				.SetParameter<int?>("WeightUnitCodeID", box.WeightUnitCodeID)
				.SetParameter<int?>("Length", box.Length)
				.SetParameter<int?>("Width", box.Width)
				.SetParameter<int?>("Depth", box.Depth)
				.SetParameter<int?>("DimensionUnitCodeID", box.DimensionUnitCodeID)
				.SetInt32("boxID", boxID)
				.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public int UpdateShipment(int customerOrderID, CustomerOrderShipment shipment)
		{
			var quertyText = $@"
				UPDATE CustomerOrderShipment SET
					  {nameof(CustomerOrderShipment.ShipCompanyType)}   = :ShipCompanyType
					, {nameof(CustomerOrderShipment.ShipMethodCodeID)}  = :ShipMethodCodeID
					, {nameof(CustomerOrderShipment.ShipAccountNumber)} = :ShipAccountNumber
					, {nameof(CustomerOrderShipment.ShippingCharge)}    = :ShippingCharge
					, {nameof(CustomerOrderShipment.FillOption)}       = :FillOption
					WHERE {nameof(CustomerOrderShipment.CustomerOrderShipmentID)} = :shipmentID
			";

			_dbContext.BeginTransaction();
			var results = _dbContext.Session.CreateSQLQuery(quertyText)
				.SetParameter<int?>("ShipCompanyType", shipment.ShipCompanyType)
				.SetParameter<int?>("ShipMethodCodeID", shipment.ShipMethodCodeID)
				.SetString("ShipAccountNumber", shipment.ShipAccountNumber)
				.SetParameter<decimal?>("ShippingCharge", shipment.ShippingCharge)
				.SetInt32("shipmentID", shipment.CustomerOrderShipmentID)
				.SetInt32("FillOption", shipment.FillOption.Value)
				.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public int UpdateShipmentInvoiceStatus(CustomerOrderShipment shipment)
		{
			var quertyText = $@"
				UPDATE CustomerOrderShipment SET
					{nameof(CustomerOrderShipment.InvoiceSent)} = :invoiceSent
					WHERE {nameof(CustomerOrderShipment.CustomerOrderShipmentID)} = :shipmentID
			";

			_dbContext.BeginTransaction();
			var results = _dbContext.Session.CreateSQLQuery(quertyText)
				.SetBoolean("invoiceSent", shipment.InvoiceSent)
				.SetInt32("shipmentID", shipment.CustomerOrderShipmentID)
				.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public int CompleteShipment(int customerOrderShipmentID, string invoiceNumber, string trackingNumber, DateTime? deliveryDate, decimal invoiceTotal)
		{
			var quertyText = $@"
				UPDATE CustomerOrderShipment SET
					 {nameof(CustomerOrderShipment.ShipmentComplete)}         = 1
					,{nameof(CustomerOrderShipment.InvoiceDate)}              = :invoiceDate
					,{nameof(CustomerOrderShipment.InvoiceNumber)}            = :invoiceNumber
					,{nameof(CustomerOrderShipment.MasterTrackingNumber)}     = :trackingNumber
					,{nameof(CustomerOrderShipment.DeliveryDate)}             = :deliveryDate
					,{nameof(CustomerOrderShipment.InvoiceTotal)}             = :invoiceTotal
				WHERE {nameof(CustomerOrderShipment.CustomerOrderShipmentID)} = :shipmentID
			";
			_dbContext.BeginTransaction();
			var results = _dbContext.Session.CreateSQLQuery(quertyText)
				.SetDateTime("invoiceDate", DateTime.UtcNow)
				.SetString("invoiceNumber", invoiceNumber)
				.SetString("trackingNumber", trackingNumber)
				.SetDecimal("invoiceTotal", invoiceTotal)
				.SetParameter<DateTime?>("deliveryDate", deliveryDate)
				.SetInt32("shipmentID", customerOrderShipmentID)
				.ExecuteUpdate();
			_dbContext.Commit();
			return results;
		}

		public IEnumerable<ReportProduct> GetCustomerOrderReportProducts(int customerOrderID)
		{
			var queryText = $@"
				SELECT
					p.{nameof(Product.ProductID)} ProductID
					,p.{nameof(Product.ProductCustomID)} ProductCustomID
					,p.{nameof(Product.ProductName)} ProductName
					,cop.{nameof(CustomerOrderProduct.Quantity)} Quantity
					,(
						SELECT ISNULL(SUM(copf.{nameof(CustomerOrderProductFill.QuantityPacked)}), 0)
						FROM CustomerOrderProductFill copf
						WHERE cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)}
					) QuantityPacked
					,cast (cop.{nameof(CustomerOrderProduct.Price)} as decimal(10,2)) Price
					,cop.{nameof(CustomerOrderProduct.Price)} Price4Decimal
				FROM CustomerOrderProduct cop
				LEFT JOIN Product p ON cop.{nameof(CustomerOrderProduct.ProductID)} = p.{nameof(Product.ProductID)}
				WHERE cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = :customerOrderID
				ORDER BY cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)} ASC
			";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("customerOrderID", customerOrderID)
				.DynamicList()
				.ToList()
				.Select(x => new ReportProduct(x));
		}

		public IEnumerable<InvoiceProduct> GetInvoiceProducts(int shipmentID)
		{
			var queryText = $@"
				SELECT
					p.{nameof(Product.ProductID)} ProductID
					,p.{nameof(Product.ProductCustomID)} ProductCustomID
					,COALESCE(copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)},-1) CustomerOrderShipmentBoxID
					,p.{nameof(Product.ProductName)} ProductName
					,cop.{nameof(CustomerOrderProduct.Quantity)} Quantity
					,COALESCE(copf.{nameof(CustomerOrderProductFill.QuantityPacked)},0) QuantityPacked
					,copf.{nameof(CustomerOrderProductFill.SerialNumbers)} SerialNumbers
					,cop.{nameof(CustomerOrderProduct.Price)} Price
				FROM CustomerOrderProduct cop
				LEFT JOIN CustomerOrderShipmentBox cosb ON cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}
				LEFT JOIN CustomerOrderProductFill copf ON cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
					 AND copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)} = cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
				LEFT JOIN Product p ON cop.{nameof(CustomerOrderProduct.ProductID)} = p.{nameof(Product.ProductID)}
				WHERE cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} = :shipmentID

			";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("shipmentID", shipmentID)
				.DynamicList()
				.ToList()
				.Select(x => new InvoiceProduct(x));
		}

		public IList<dynamic> GetPreviousePeachtreeInvoices(int top=10)
		{
			var queryText =$@"SELECT TOP ({top}) [AccountingExportBatchID]
							  ,[ExportTS]
						  FROM [dbo].[AccountingExportBatch]
						  Order By ExportTS desc";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.DynamicList();
		}

		public IList<dynamic> GetPreviousPeachTreeInvoicesByDate(string StartDate, string EndDate)
		{
			var queryText = $@"SELECT [AccountingExportBatchID]
							  ,[ExportTS]
						  FROM [dbo].[AccountingExportBatch]
						WHERE ExportTS >= :StartDate AND ExportTS <= :EndDate
						  Order By ExportTS desc";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetString("StartDate", StartDate)
				.SetString("EndDate", EndDate)
				.DynamicList();
		}

		public IList<dynamic> GetPreviousPeachtreeInvoicesByBatchId(int BatchId)
		{
			return _dbContext.Session
				.CreateSQLQuery("EXEC GeneratePeachTreeInvoiceByBatchId @newBatchID = :newBatchID")
				.SetInt32("newBatchID", BatchId)
				.DynamicList();
		}
		
		public IList<dynamic> GetPeachTreeInvoiceTaxByBatchId(int BatchId)
		{
			var queryText = $@"select  distinct 
			c.CustomerCustomID as [Customer ID]
	, s.InvoiceNumber as [Invoice/CM #] 
	, FORMAT (isnull(s.InvoiceDate,getdate()), 'MM-dd-yyyy') as [Date]
	, loc.name as [Ship to Name]
	, replace(loc.address,',',' ') as [Ship to Address-Line One]
	, '' as [Ship to Address-Line Two]
	, loc.city as [Ship to City]
	, isnull(statecodes.codeName, '') as [Ship to State]
	, loc.zipcode as [Ship to Zipcode]
	, countrycodes.codedescription as [Ship to Country]
	, co.PONumber as [Customer PO]
	, shipper.CodeDescription as [Ship Via]
	, FORMAT (s.CreatedOn, 'MM-dd-yyyy') as [Ship Date]
	, FORMAT (dateadd(day, isnull(c.paymenttermsnetduedays,0), isnull(s.InvoiceDate, getdate())) , 'MM-dd-yyyy') as [Date Due]
	, case c.paymenttermstype when 1 then 'COD' when 2 then 'Prepay' when 3 then 'Net ' + convert(varchar,c.paymenttermsnetduedays) + ' days' end as [Displayed Terms]
	, u.SalesRepID as [Sales Representative ID]
	, credit.CodeName as [Accounts Receivable Account]
	, isnull(co.notes,'') as [Invoice Note]
	, 'FALSE' as [Note Prints After Line Items]
	, 2+(select count(*) from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID) as [Number of Distributions]
	, (select count(*)+2 from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID ) as [Invoice/CM Distribution]
	, '' as [Quantity]
	, '' as [Item ID]
	, '' as [Serial Number]
	, '' as [Description]
	, '65000-01' as [G/L Account]
	, 0 as [Unit Price]
	, case when salestaxcodes.CodeDescription='exempt' then 2 else 1 end as [Tax Type]
	, case when salestaxcodes.CodeDescription='exempt' then 0 else .08 end * -1 * (select sum(cop.quantity * cop.price) from customerorderproductfill f2 inner join customerorderproduct cop on cop.CustomerOrderProductID=f2.CustomerOrderProductID where f2.customerordershipmentboxid=b.customerordershipmentboxid) as [Amount]
	, '' as [U/M ID]
	, 1 as [U/M No. of Stocking Units]
	, LEFT(salestaxcodes.CodeDescription,8) as [Sales Tax ID] 
	, LEFT(salestaxcodes.CodeDescription,8) as [Sales Tax Agency ID]

	from customer c
	left join code salestaxcodes on salestaxcodes.codeid=c.salestaxcodeid

	left join code debit on debit.codeid=c.GLSalesCodeID --if order or if invoiced
	left join code credit on credit.codeid=c.GLAccountsReceivableCodeID --invoiced

	inner join customerorder co on co.customerid=c.customerid
	inner join customerordershipment s on s.customerorderid=co.customerorderid
	inner join customerordershipmentbox b on b.customerordershipmentid=s.customerordershipmentid

	inner join code shipper on shipper.codeid=s.shipcompanytype

	inner join customershippinginfo loc on loc.customershippinginfoid=co.customershippinginfoid
	left join code statecodes on statecodes.codeid=loc.statecodeid
	inner join code countrycodes on countrycodes.codeid=loc.countrycodeid
	inner join [user] u on u.userid=loc.repuserid


	where s.accountingexportbatchid = :newBatchID";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("newBatchID", BatchId)
				.DynamicList();
		}

		public IList<dynamic> GetPeachTreeInvoiceShippingByBatchId(int BatchId)
		{
			var queryText = $@"select distinct
			c.CustomerCustomID as [Customer ID]
	, s.InvoiceNumber as [Invoice/CM #] 
	, FORMAT (isnull(s.InvoiceDate,getdate()), 'MM-dd-yyyy') as [Date]
	, loc.name as [Ship to Name]
	, replace(loc.address,',',' ') as [Ship to Address-Line One]
	, '' as [Ship to Address-Line Two]
	, loc.city as [Ship to City]
	, isnull(statecodes.codeName, '') as [Ship to State]
	, loc.zipcode as [Ship to Zipcode]
	, countrycodes.codedescription as [Ship to Country]
	, co.PONumber as [Customer PO]
	, shipper.CodeDescription as [Ship Via]
	, FORMAT (s.CreatedOn, 'MM-dd-yyyy') as [Ship Date]
	, FORMAT (dateadd(day, isnull(c.paymenttermsnetduedays,0), isnull(s.InvoiceDate, getdate())) , 'MM-dd-yyyy') as [Date Due]
	, case c.paymenttermstype when 1 then 'COD' when 2 then 'Prepay' when 3 then 'Net ' + convert(varchar,c.paymenttermsnetduedays) + ' days' end as [Displayed Terms]
	, u.SalesRepID as [Sales Representative ID]
	, credit.CodeName as [Accounts Receivable Account]
	, isnull(co.notes,'') as [Invoice Note]
	, 'FALSE' as [Note Prints After Line Items]
	, 2+(select count(*) from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID) as [Number of Distributions]
	, 0 as [Invoice/CM Distribution]
	, '' as [Quantity]
	, '' as [Item ID]
	, '' as [Serial Number]
	, 'Freight Amount' as [Description]
	, debit.codename as [G/L Account]
	, '0' as [Unit Price]
	, 26 as [Tax Type]
    --, 2 as [Tax Type]
	, -1 * s.shippingcharge as [Amount]
	, '' as [U/M ID]
	, 1 as [U/M No. of Stocking Units]
	, '' as [Sales Tax ID] 
	, '' as [Sales Tax Agency ID]

	from customer c
	left join code salestaxcodes on salestaxcodes.codeid=c.salestaxcodeid

	left join code debit on debit.codeid=c.glshippingchargecodeid --if order or if invoiced
	left join code credit on credit.codeid=c.GLAccountsReceivableCodeID --invoiced

	inner join customerorder co on co.customerid=c.customerid
	inner join customerordershipment s on s.customerorderid=co.customerorderid
	inner join customerordershipmentbox b on b.customerordershipmentid=s.customerordershipmentid

	inner join code shipper on shipper.codeid=s.shipcompanytype

	inner join customershippinginfo loc on loc.customershippinginfoid=co.customershippinginfoid
	left join code statecodes on statecodes.codeid=loc.statecodeid
	inner join code countrycodes on countrycodes.codeid=loc.countrycodeid
	inner join [user] u on u.userid=loc.repuserid

	where s.accountingexportbatchid = :newBatchID";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("newBatchID", BatchId)
				.DynamicList();
		}

		public IList<dynamic> GetPeachTreeInvoiceCreditCardFeeByBatchId(int BatchId)
		{
			var queryText = $@"select  distinct
			c.CustomerCustomID as [Customer ID]
	, s.InvoiceNumber as [Invoice/CM #] 
	, FORMAT (isnull(s.InvoiceDate,getdate()), 'MM-dd-yyyy') as [Date]
	, loc.name as [Ship to Name]
	, replace(loc.address,',',' ') as [Ship to Address-Line One]
	, '' as [Ship to Address-Line Two]
	, loc.city as [Ship to City]
	, isnull(statecodes.codeName, '') as [Ship to State]
	, loc.zipcode as [Ship to Zipcode]
	, countrycodes.codedescription as [Ship to Country]
	, co.PONumber as [Customer PO]
	, shipper.CodeDescription as [Ship Via]
	, FORMAT (s.CreatedOn, 'MM-dd-yyyy') as [Ship Date]
	, FORMAT (dateadd(day, isnull(c.paymenttermsnetduedays,0), isnull(s.InvoiceDate, getdate())) , 'MM-dd-yyyy') as [Date Due]
	, case c.paymenttermstype when 1 then 'COD' when 2 then 'Prepay' when 3 then 'Net ' + convert(varchar,c.paymenttermsnetduedays) + ' days' end as [Displayed Terms]
	, u.SalesRepID as [Sales Representative ID]
	, CASE WHEN c.CustomerCustomID LIKE '1-%' THEN '12000-01' WHEN c.CustomerCustomID LIKE '2-%' THEN '12000-02' WHEN c.CustomerCustomID LIKE '3-%' THEN '12000-03' ELSE '12000-04' END as [Accounts Receivable Account]
	, isnull(co.notes,'') as [Invoice Note]
	, 'FALSE' as [Note Prints After Line Items]
	, 2+(select count(*) from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID) as [Number of Distributions]
	, (select count(*)+1 from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID ) as [Invoice/CM Distribution]
	, '' as [Quantity]
	, CASE WHEN c.CustomerCustomID LIKE '1-%' OR c.CustomerCustomID LIKE '2-%' THEN 'CCF-1' ELSE 'CCF-3' END as [Item ID]
	, '' as [Serial Number]
	, 'Credit Card Fee' as [Description]
	, CASE WHEN c.CustomerCustomID LIKE '1-%' OR c.CustomerCustomID LIKE '2-%' THEN '54200-01' ELSE '54200-03' END as [G/L Account]
	, '0' as [Unit Price]
	, 2 as [Tax Type]
    --, 2 as [Tax Type]
	, 0.03 *(select SUM(Amount) from @temp where ""Invoice/CM #"" = s.InvoiceNumber) as [Amount]
	, '' as [U/M ID]
	, 1 as [U/M No. of Stocking Units]
	, '' as [Sales Tax ID] 
	, '' as [Sales Tax Agency ID]

	from customer c
	left join code salestaxcodes on salestaxcodes.codeid=c.salestaxcodeid

	left join code debit on debit.codeid=c.glshippingchargecodeid --if order or if invoiced
	left join code credit on credit.codeid=c.GLAccountsReceivableCodeID --invoiced

	inner join customerorder co on co.customerid=c.customerid
	inner join customerordershipment s on s.customerorderid=co.customerorderid
	inner join customerordershipmentbox b on b.customerordershipmentid=s.customerordershipmentid

	inner join code shipper on shipper.codeid=s.shipcompanytype

	inner join customershippinginfo loc on loc.customershippinginfoid=co.customershippinginfoid
	left join code statecodes on statecodes.codeid=loc.statecodeid
	inner join code countrycodes on countrycodes.codeid=loc.countrycodeid
	inner join [user] u on u.userid=loc.repuserid

	where s.accountingexportbatchid = @:newBatchID";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("newBatchID", BatchId)
				.DynamicList();
		}

		public IList<dynamic> GetPeachTreeInvoiceContents()
		{
			return _dbContext.Session
				.CreateSQLQuery("EXEC GeneratePeachTreeInvoice")
				.DynamicList();
		}

		public int SetFillingStatus(int customerOrderID, int currentUserId, bool isFilling)
		{
			var queryText = $@"
				UPDATE CustomerOrder SET {nameof(CustomerOrder.IsFilling)} = :fillingStatus,
										 {nameof(CustomerOrder.FilledBy)} = :filledBy,
										 {nameof(CustomerOrder.FilledByOn)} = :filledByOn
				WHERE {nameof(CustomerOrder.CustomerOrderID)} = :customerOrderID
			";

			_dbContext.BeginTransaction();
			var result = _dbContext.Session.CreateSQLQuery(queryText)
				.SetInt32("filledBy", currentUserId)
				.SetDateTime("filledByOn", DateTime.UtcNow)
				.SetInt32("customerOrderID", customerOrderID)
				.SetInt32("fillingStatus", isFilling ? 1 : 0)
				.ExecuteUpdate();
			_dbContext.Commit();
			return result;
		}

		public int SetShipmentStatus(int customerOrderID, int currentUserId)
		{
            var queryText = $@"
				UPDATE CustomerOrder SET {nameof(CustomerOrder.ShippedBy)} = :shippedBy,
										 {nameof(CustomerOrder.ShippedByOn)} = :shippedOn
				WHERE {nameof(CustomerOrder.CustomerOrderID)} = :customerOrderID
			";

            _dbContext.BeginTransaction();
            var result = _dbContext.Session.CreateSQLQuery(queryText)
                .SetInt32("shippedBy", currentUserId)
                .SetDateTime("shippedOn", DateTime.UtcNow)
                .SetInt32("customerOrderID", customerOrderID)
                .ExecuteUpdate();
            _dbContext.Commit();
            return result;
        }
			
        public Task<IList<dynamic>> GetArchivedInvoiceList(string search, string sortCol, bool sortAsc, int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
        {
			var hasCustomerShipData = true;
			var startingTable = $@"
						CustomerOrderShipment ship
						JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
					";

			var additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, ship.{nameof(CustomerOrderShipment.InvoiceDate)}
						, ship.{nameof(CustomerOrderShipment.InvoiceNumber)}
						, ship.{nameof(CustomerOrderShipment.InvoiceTotal)}
						, ship.{nameof(CustomerOrderShipment.MasterTrackingNumber)}	
						, co.{nameof(CustomerOrder.ShippedBy)}
						, co.{nameof(CustomerOrder.ShippedByOn)}
                        , co.{nameof(CustomerOrder.FilledBy)}
				        , co.{nameof(CustomerOrder.FilledByOn)}	
						, CAST (1 as BIT) HasFilled
						, CAST (1 as BIT) HasShipped
					";
			var statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 1 AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 1";

            var now = DateTime.UtcNow;
            var minDate = DateTime.MinValue;
            var maxDate = now.AddDays(-30).Date;

            statusFilter += $@"
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} >= :minDate
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} < :maxDate
				";


            var queryText = $@"
				SELECT
					co.{nameof(CustomerOrder.CustomerOrderID)}
					,co.{nameof(CustomerOrder.SubmitDate)}
					,co.{nameof(CustomerOrder.CustomerOrderCustomID)}
					,c.{nameof(Customer.CustomerCustomID)}
					,c.{nameof(Customer.CustomerName)}
					,csi.{nameof(CustomerShippingInfo.Name)} as LocationName
					,csi.{nameof(CustomerShippingInfo.City)} as LocationCity
					,co.{nameof(CustomerOrder.PONumber)}
					,_cop.Subtotal
					,u.{nameof(User.SalesRepID)} + ' ' + u.{nameof(User.FirstName)} + ' ' + u.{nameof(User.LastName)} as SalesRep
					,co.{nameof(CustomerOrder.MGApprovedOn)}
					,co.{nameof(CustomerOrder.VPApprovedOn)}
					,_box.QuantityPacked
					,_cop.Quantity
					,co.{nameof(CustomerOrder.IsDoNotFill)}
					,co.{nameof(CustomerOrder.AttachmentURI)}
					
					{additionalColumns}
				FROM {startingTable}
				LEFT JOIN Customer c on c.{nameof(Customer.CustomerID)} = co.{nameof(CustomerOrder.CustomerID)}
				LEFT JOIN CustomerShippingInfo csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
				LEFT JOIN [User] u on u.{nameof(User.UserId)} = csi.{nameof(CustomerShippingInfo.RepUserID)}
				LEFT JOIN (
					SELECT
						{nameof(CustomerOrderProduct.CustomerOrderID)}
						,SUM({nameof(CustomerOrderProduct.Quantity)}) as Quantity
						,SUM({nameof(CustomerOrderProduct.Quantity)} * {nameof(CustomerOrderProduct.Price)}) as Subtotal
						,STRING_AGG(p.{nameof(Product.ProductName)}, ',') as ItemNames
						,STRING_AGG(p.{nameof(Product.ProductCustomID)}, ',') as ItemNums
					FROM CustomerOrderProduct cop
					JOIN Product p on p.{nameof(Product.ProductID)} = cop.{nameof(CustomerOrderProduct.ProductID)}
					GROUP BY CustomerOrderID
				) _cop ON _cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
				LEFT JOIN (
					SELECT box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}, SUM(copf.{nameof(CustomerOrderProductFill.QuantityPacked)}) QuantityPacked
					FROM CustomerOrderProductFill copf
					JOIN CustomerOrderShipmentBox box ON box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
					WHERE box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} IS NOT NULL
					GROUP BY box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}
				) _box ON _box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}

				WHERE {statusFilter}
			";

			if (!showAll)
			{
				var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
				if (showInternational)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
				}
				if (showDomesticDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
				}
				if (showDomesticNonDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
				}
				roleFilter += ")";
				queryText += roleFilter;
			}
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				var moreSearchFilters = string.Empty;
				if (hasCustomerShipData)
				{
					moreSearchFilters = $@"
						OR ship.{nameof(CustomerOrderShipment.InvoiceDate)} LIKE :searchTerm
						OR ship.{nameof(CustomerOrderShipment.InvoiceNumber)} LIKE :searchTerm
					";
				}
				queryText += $@"
					AND (
						c.{nameof(Customer.CustomerCustomID)} LIKE :searchTerm
						OR co.{nameof(CustomerOrder.PONumber)} LIKE :searchTerm
						OR {nameof(CustomerOrder.CustomerOrderCustomID)} LIKE :searchTerm
						OR {nameof(CustomerOrderShipment.MasterTrackingNumber)} LIKE :searchTerm
						{moreSearchFilters}
					)
				";
			}

			var sortProp = new List<string>(){
				"CustomerOrderShipmentID",
				"CustomerOrderID",
				"CustomerCustomID",
				"InvoiceDate",
				"InvoiceNumber",
				"CustomerOrderCustomID",
				"PONumber"
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

			if (!sortProp.IsNullOrEmpty())
				queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				query.SetString("searchTerm", $"%{search}%");
			}
			query.SetDateTime("minDate", minDate);
			query.SetDateTime("maxDate", maxDate);

			return query.DynamicListAsync();
		}

		public Task<IList<dynamic>> GetInvoiceActivity(string search, string sortCol, bool sortAsc, int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, DateTime startDate, DateTime endDate)
		{
			var hasCustomerShipData = true;
			var startingTable = $@"
						CustomerOrderShipment ship
						JOIN CustomerOrder co ON co.{nameof(CustomerOrder.CustomerOrderID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
						
					";

			var additionalColumns = $@"
						, ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
						, ship.{nameof(CustomerOrderShipment.InvoiceDate)}
						, ship.{nameof(CustomerOrderShipment.InvoiceNumber)}
						, ship.{nameof(CustomerOrderShipment.InvoiceTotal)}
						, ship.{nameof(CustomerOrderShipment.MasterTrackingNumber)}	
						, co.{nameof(CustomerOrder.PONumber)}
						, co.{nameof(CustomerOrder.Contact)}
                        , c.{nameof(Customer.PrimaryEmail)}
				        , c.{nameof(Customer.PrimaryPhone)}
						,c.{nameof(Customer.CustomerName)}
						, ct.{nameof(CodeType.CodeTypeName)} as Practice
						, CAST (1 as BIT) HasFilled
						, CAST (1 as BIT) HasShipped
					";
			var statusFilter = $"ship.{nameof(CustomerOrderShipment.ShipmentComplete)} = 1 AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 1";


			statusFilter += $@"
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} >= :minDate
					AND ship.{nameof(CustomerOrderShipment.InvoiceDate)} < :maxDate
					AND ship.{nameof(CustomerOrderShipment.InvoiceSent)} = 1
				";


			var queryText = $@"
				SELECT
					co.{nameof(CustomerOrder.CustomerOrderID)}
					,co.{nameof(CustomerOrder.SubmitDate)}
					,co.{nameof(CustomerOrder.CustomerOrderCustomID)}
					,c.{nameof(Customer.CustomerCustomID)}
					,c.{nameof(Customer.CustomerName)}
					,csi.{nameof(CustomerShippingInfo.Name)} as LocationName
					,csi.{nameof(CustomerShippingInfo.City)} as LocationCity
					,co.{nameof(CustomerOrder.PONumber)}
					,_cop.Subtotal
					,u.{nameof(User.SalesRepID)} + ' ' + u.{nameof(User.FirstName)} + ' ' + u.{nameof(User.LastName)} as SalesRep
					,co.{nameof(CustomerOrder.MGApprovedOn)}
					,co.{nameof(CustomerOrder.VPApprovedOn)}
					,_box.QuantityPacked
					,_cop.Quantity
					,co.{nameof(CustomerOrder.IsDoNotFill)}
					,co.{nameof(CustomerOrder.AttachmentURI)}
					
					{additionalColumns}
				FROM {startingTable}
				LEFT JOIN Customer c on c.{nameof(Customer.CustomerID)} = co.{nameof(CustomerOrder.CustomerID)}
				LEFT JOIN CustomerShippingInfo csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
				LEFT JOIN [User] u on u.{nameof(User.UserId)} = csi.{nameof(CustomerShippingInfo.RepUserID)}
				LEFT JOIN Code code ON c.{nameof(Customer.PracticeTypeCodeID)} = code.{nameof(Code.CodeID)}
				INNER JOIN CodeType ct on code.{nameof(Code.CodeTypeID)} = ct.{nameof(CodeType.CodeTypeID)}
				LEFT JOIN (
					SELECT
						{nameof(CustomerOrderProduct.CustomerOrderID)}
						,SUM({nameof(CustomerOrderProduct.Quantity)}) as Quantity
						,SUM({nameof(CustomerOrderProduct.Quantity)} * {nameof(CustomerOrderProduct.Price)}) as Subtotal
						,STRING_AGG(p.{nameof(Product.ProductName)}, ',') as ItemNames
						,STRING_AGG(p.{nameof(Product.ProductCustomID)}, ',') as ItemNums
					FROM CustomerOrderProduct cop
					JOIN Product p on p.{nameof(Product.ProductID)} = cop.{nameof(CustomerOrderProduct.ProductID)}
					GROUP BY CustomerOrderID
				) _cop ON _cop.{nameof(CustomerOrderProduct.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
				LEFT JOIN (
					SELECT box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}, SUM(copf.{nameof(CustomerOrderProductFill.QuantityPacked)}) QuantityPacked
					FROM CustomerOrderProductFill copf
					JOIN CustomerOrderShipmentBox box ON box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
					WHERE box.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} IS NOT NULL
					GROUP BY box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)}
				) _box ON _box.{nameof(CustomerOrderShipmentBox.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}

				WHERE {statusFilter}
			";

			if (!showAll)
			{
				var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
				if (showInternational)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
				}
				if (showDomesticDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
				}
				if (showDomesticNonDistributors)
				{
					roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
				}
				roleFilter += ")";
				queryText += roleFilter;
			}
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				var moreSearchFilters = string.Empty;
				if (hasCustomerShipData)
				{
					moreSearchFilters = $@"
						OR ship.{nameof(CustomerOrderShipment.InvoiceDate)} LIKE :searchTerm
						OR ship.{nameof(CustomerOrderShipment.InvoiceNumber)} LIKE :searchTerm
					";
				}
				queryText += $@"
					AND (
						c.{nameof(Customer.CustomerCustomID)} LIKE :searchTerm
						OR co.{nameof(CustomerOrder.PONumber)} LIKE :searchTerm
						OR {nameof(CustomerOrder.CustomerOrderCustomID)} LIKE :searchTerm
						OR {nameof(CustomerOrderShipment.MasterTrackingNumber)} LIKE :searchTerm
						{moreSearchFilters}
					)
				";
			}

			var sortProp = new List<string>(){
				"CustomerOrderShipmentID",
				"CustomerOrderID",
				"CustomerCustomID",
				"InvoiceDate",
				"InvoiceNumber",
				"CustomerOrderCustomID",
				"PONumber"
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

			if (!sortProp.IsNullOrEmpty())
				queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				query.SetString("searchTerm", $"%{search}%");
			}
			query.SetDateTime("minDate", startDate);
			query.SetDateTime("maxDate", endDate);

			return query.DynamicListAsync();
		}

		public async Task<IList<dynamic>> GetFilteredProductShippedList(string search, string sortCol, bool sortAsc, int currentUserID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			dynamic data = JsonConvert.DeserializeObject(search);
			CultureInfo provider = CultureInfo.InvariantCulture;
			DateTime minDate;
			DateTime maxDate;
			DateTime.TryParseExact(
			data.minDate.ToString(),
			"yyyy-MM-dd",
			System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.None,
			out minDate);
			DateTime.TryParseExact(
			data.maxDate.ToString(),
			"yyyy-MM-dd",
			System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.None,
			out maxDate);

			var queryText = $@"
				SELECT
					ship.{nameof(CustomerOrderShipment.InvoiceDate)}
					, ship.{nameof(CustomerOrderShipment.InvoiceNumber)}
					, ship.{nameof(CustomerOrderShipment.InvoiceTotal)}
					, ship.{nameof(CustomerOrderShipment.CustomerOrderID)}
					, co.{nameof(CustomerOrder.CustomerOrderCustomID)}
					, co.{nameof(CustomerOrder.PONumber)}
					, co.{nameof(CustomerOrder.ShippedBy)}
		            , co.{nameof(CustomerOrder.ShippedByOn)}
					, co.{nameof(CustomerOrder.FilledBy)}	
					, co.{nameof(CustomerOrder.FilledByOn)}
					, c.{nameof(Customer.CustomerCustomID)}
					,c.{nameof(Customer.IsInternational)}
					,c.{nameof(Customer.IsDomesticDistributor)}
					,c.{nameof(Customer.IsDomestic)}
					,c.{nameof(Customer.IsDomesticAfaxys)}
					, p.{nameof(Product.ProductCustomID)}
	                , p.{nameof(Product.ProductName)}		
					, copf.{nameof(CustomerOrderProductFill.QuantityPacked)} as Quantity
					, cop.{nameof(CustomerOrderProduct.Price)}
				FROM
					CustomerOrderShipment ship
					LEFT JOIN CustomerOrderShipmentBox cosb ON cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentID)} = ship.{nameof(CustomerOrderShipment.CustomerOrderShipmentID)}
					LEFT JOIN CustomerOrderProductFill copf ON cosb.{nameof(CustomerOrderShipmentBox.CustomerOrderShipmentBoxID)} = copf.{nameof(CustomerOrderProductFill.CustomerOrderShipmentBoxID)}
					LEFT JOIN CustomerOrderProduct cop ON	copf.{nameof(CustomerOrderProductFill.CustomerOrderProductID)} = cop.{nameof(CustomerOrderProduct.CustomerOrderProductID)}
					LEFT JOIN Product p ON cop.{nameof(CustomerOrderProduct.ProductID)} = p.{nameof(Product.ProductID)}
					LEFT JOIN CustomerOrder co ON ship.{nameof(CustomerOrderShipment.CustomerOrderID)} = co.{nameof(CustomerOrder.CustomerOrderID)}
					LEFT JOIN Customer c ON co.{nameof(CustomerOrder.CustomerID)} = c.{nameof(Customer.CustomerID)}
					LEFT JOIN CustomerShippingInfo csi on csi.{nameof(CustomerShippingInfo.CustomerShippingInfoID)} = co.{nameof(CustomerOrder.CustomerShippingInfoID)}
				WHERE
					CONVERT(DATE, ship.InvoiceDate) >= :minDate AND CONVERT(DATE, ship.InvoiceDate) <= :maxDate";
				
				if(data.productID != null && data.productID > 0)
					queryText += $" AND p.{nameof(Product.ProductCustomID)} = :productID";
				else
					queryText += $" AND p.{nameof(Product.ProductCustomID)} <> :productID";

            if (!string.IsNullOrEmpty(data.productName.ToString()))
                queryText += $" AND p.{nameof(Product.ProductName)} LIKE :productName";
            else
                queryText += $" AND p.{nameof(Product.ProductCustomID)} <> :productName";

            if (!showAll)
            {
                var roleFilter = $@"
					AND (
						(csi.{nameof(CustomerShippingInfo.RepUserID)} = {currentUserID}) ";
                if (showInternational)
                {
                    roleFilter += $@"
						OR (c.{nameof(Customer.IsInternational)} = 1) ";
                }
                if (showDomesticDistributors)
                {
                    roleFilter += $@"
						OR (c.{nameof(Customer.IsDomesticDistributor)} = 1) ";
                }
                if (showDomesticNonDistributors)
                {
                    roleFilter += $@"
						OR (c.{nameof(Customer.IsDomestic)} = 1 OR c.{nameof(Customer.IsDomesticAfaxys)} = 1) ";
                }
                roleFilter += ")";
                queryText += roleFilter;
            }

            var sortProp = new List<string>(){
				"CustomerOrderID",
				"InvoiceDate",
				"InvoiceNumber",
				"PONumber"
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());

			if (!sortProp.IsNullOrEmpty())
				queryText += $" ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";
			try
			{
				var query = _dbContext.Session.CreateSQLQuery(queryText);
				query.SetDateTime("minDate", minDate);
				query.SetDateTime("maxDate", maxDate);
				query.SetString("productID", data.productID.ToString());
                query.SetString("productName", $"%{data.productName.ToString()}%");
                Task<IList<dynamic>> output = query.DynamicListAsync();
				return await output;
			} catch(Exception e)
            {
				Console.WriteLine(e);
				Console.WriteLine("outt");
				return null;
            }
		}
	}
}
