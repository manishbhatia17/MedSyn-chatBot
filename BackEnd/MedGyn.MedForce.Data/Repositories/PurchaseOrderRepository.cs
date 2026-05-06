using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Data.Repositories
{
	public class PurchaseOrderRepository : IPurchaseOrderRepository
	{
		private readonly IDbContext _dbContext;
		private readonly ConnectionStrings _connectionStrings;
		public PurchaseOrderRepository(IDbContext dbContext, IOptions<ConnectionStrings> connectionStringOptions)
		{
			_dbContext = dbContext;
			_connectionStrings = connectionStringOptions.Value;
		}

		public IList<dynamic> GetPurchaseOrderList(string search, string sortCol, bool sortAsc, PurchaseOrderStatusEnum status,
			DateRangeEnum timeframe, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors, int userId)
		{
			var statusFilter = "1 = 1";
			switch(status) {
				case PurchaseOrderStatusEnum.WaitingSubmission:
					statusFilter = $"po.{nameof(PurchaseOrder.SubmitDate)} IS NULL";
					break;
				case PurchaseOrderStatusEnum.WaitingApproval:
					statusFilter = $"po.{nameof(PurchaseOrder.SubmitDate)} IS NOT NULL AND po.{nameof(PurchaseOrder.ApprovalDate)} IS NULL";
					break;
				case PurchaseOrderStatusEnum.Receivable:
					statusFilter = $@"
						po.{nameof(PurchaseOrder.ApprovalDate)} IS NOT NULL
						AND po.{nameof(PurchaseOrder.IsDoNotReceive)} = 0
						AND (TotalReceived IS NULL OR TotalReceived < Items)
					";
					break;
				case PurchaseOrderStatusEnum.DoNotReceive:
					statusFilter = $"po.{nameof(PurchaseOrder.ApprovalDate)} IS NOT NULL AND po.{nameof(PurchaseOrder.IsDoNotReceive)} = 1";
					break;
				case PurchaseOrderStatusEnum.HasBeenReceived:
					statusFilter = $@"
						po.{nameof(PurchaseOrder.ApprovalDate)} IS NOT NULL
						AND po.{nameof(PurchaseOrder.IsDoNotReceive)} = 0
						AND TotalReceived = Items
					";
					break;
				case PurchaseOrderStatusEnum.MyPOs:
					statusFilter = $@"
                        po.{nameof(PurchaseOrder.IsDoNotReceive)} = 0
                        AND (TotalReceived IS NULL OR TotalReceived < Items)
                        AND po.{nameof(PurchaseOrder.CreatedBy)} = {userId}";
					break;
			}

			var minDate = DateTime.MinValue;
			var maxDate = DateTime.MaxValue;
			var now = DateTime.UtcNow;
			switch (timeframe) {
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

			if (timeframe != DateRangeEnum.None)
			{
				statusFilter += $@"
					AND poprTotals.LastReceived >= :minDate
					AND poprTotals.LastReceived < :maxDate
				";
			}

			var queryText = $@"
				SELECT
					po.{nameof(PurchaseOrder.PurchaseOrderID)}
					,po.{nameof(PurchaseOrder.PurchaseOrderCustomID)}
					,po.{nameof(PurchaseOrder.VendorOrderNumber)}
					,v.{nameof(Vendor.VendorCustomID)}
					,v.{nameof(Vendor.VendorName)}
					,v.{nameof(Vendor.StateCodeID)}
					,po.{nameof(PurchaseOrder.ExpectedDate)}
	                   ,po.{nameof(PurchaseOrder.SubmitDate)}
					   ,po.{nameof(PurchaseOrder.ApprovalDate)}
                          ,po.{nameof(PurchaseOrder.IsDoNotReceive)}	
					,popTotals.Items
					,popTotals.Amount
					,po.{nameof(PurchaseOrder.ShippingCharge)}
					,poprTotals.TotalReceived
					,p.{nameof(Product.ProductCustomID)} PrimaryProductCustomID
					,popTotals.Quantity PrimaryProductCount
,poprLOT.PrimaryLotNumber
				FROM PurchaseOrder po
				LEFT JOIN Vendor v on v.{nameof(Vendor.VendorID)} = po.{nameof(PurchaseOrder.VendorID)}
				LEFT JOIN (
					SELECT
						pop.{nameof(PurchaseOrderProduct.PurchaseOrderID)}
						,pop.{nameof(PurchaseOrderProduct.ProductID)}
						,t.Items
						,t.Amount
						,t.Quantity
					FROM PurchaseOrderProduct pop
					INNER JOIN (
						SELECT
							{nameof(PurchaseOrderProduct.PurchaseOrderID)}
							,SUM({nameof(PurchaseOrderProduct.Quantity)}) Items
							,SUM({nameof(PurchaseOrderProduct.Price)} * {nameof(PurchaseOrderProduct.Quantity)}) Amount
							,MAX({nameof(PurchaseOrderProduct.Quantity)}) Quantity
						FROM PurchaseOrderProduct
						GROUP BY {nameof(PurchaseOrderProduct.PurchaseOrderID)}
					) t ON t.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = pop.{nameof(PurchaseOrderProduct.PurchaseOrderID)} AND t.Quantity = pop.{nameof(PurchaseOrderProduct.Quantity)}
				) popTotals ON popTotals.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = po.{nameof(PurchaseOrder.PurchaseOrderID)}
				LEFT JOIN (
					SELECT
						pop2.{nameof(PurchaseOrderProduct.PurchaseOrderID)} {nameof(PurchaseOrderProduct.PurchaseOrderID)}
						,SUM(popr.{nameof(PurchaseOrderProductReceipt.QuantityReceived)}) TotalReceived
						,MAX(popr.{nameof(PurchaseOrderProductReceipt.ReceiptDate)}) LastReceived
					FROM PurchaseOrderProductReceipt popr
					INNER JOIN PurchaseOrderProduct pop2
						ON pop2.{nameof(PurchaseOrderProduct.PurchaseOrderProductID)} = popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
					GROUP BY pop2.{nameof(PurchaseOrderProduct.PurchaseOrderID)}
				) poprTotals ON poprTotals.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = po.{nameof(PurchaseOrder.PurchaseOrderID)}
				LEFT JOIN (
					SELECT TOP 1000
						pop2.{nameof(PurchaseOrderProduct.PurchaseOrderID)} {nameof(PurchaseOrderProduct.PurchaseOrderID)}
						,popr.{nameof(PurchaseOrderProductReceipt.SerialNumbers)} PrimaryLotNumber
						,popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
					FROM PurchaseOrderProductReceipt popr
					INNER JOIN PurchaseOrderProduct pop2
						ON pop2.{nameof(PurchaseOrderProduct.PurchaseOrderProductID)} = popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
                    ORDER BY popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)} ASC
				) poprLOT ON poprLOT.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = po.{nameof(PurchaseOrder.PurchaseOrderID)}
				LEFT JOIN Product p ON p.{nameof(Product.ProductID)} = popTotals.{nameof(PurchaseOrderProduct.ProductID)}
				WHERE {statusFilter}
			";
			if (!showAll && status != PurchaseOrderStatusEnum.Receivable && status != PurchaseOrderStatusEnum.HasBeenReceived && status != PurchaseOrderStatusEnum.DoNotReceive)
			{
				var roleFilter = $@"
				AND ( 1=0 ";
				if (showInternational)
				{
					roleFilter += $@"
					OR (v.{nameof(Vendor.IsInternational)} = 1) ";
				}
				if (showDomesticDistributors)
				{
					roleFilter += $@"
					OR (v.{nameof(Vendor.IsDomesticDistributor)} = 1) ";
				}
				if (showDomesticNonDistributors)
				{
					roleFilter += $@"
					OR (v.{nameof(Vendor.IsDomestic)} = 1 OR v.{nameof(Vendor.IsDomesticAfaxys)} = 1) ";
				}
				roleFilter += ")";
				queryText += roleFilter;
			}
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				queryText += $@"
					AND (
						po.{nameof(PurchaseOrder.PurchaseOrderCustomID)} LIKE :searchTerm
						OR v.{nameof(Vendor.VendorName)} LIKE :searchTerm
						OR p.{nameof(Product.ProductName)} LIKE :searchTerm
						OR po.{nameof(PurchaseOrder.VendorOrderNumber)} LIKE :searchTerm
						OR EXISTS (
							SELECT 1
							FROM PurchaseOrderProduct pop2
							INNER JOIN Product p2 ON p2.ProductID = pop2.ProductID
							WHERE pop2.PurchaseOrderID = po.PurchaseOrderID
							  AND p2.ProductCustomID LIKE :searchTerm
						)
					)
				";
			}

			var sortProp = new List<string>(){
				nameof(PurchaseOrder.PurchaseOrderID),
				nameof(PurchaseOrder.PurchaseOrderCustomID),
				nameof(Vendor.VendorCustomID),
				nameof(Vendor.VendorName),
				nameof(PurchaseOrder.ExpectedDate),
				"Items",
				"Amount",
				"PrimaryProductCustomID",
				"PrimaryProductCount",
				nameof(PurchaseOrder.VendorOrderNumber)
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());
			queryText += $"ORDER BY {sortProp} {(sortAsc ? "ASC" : "DESC")}";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				query.SetString("searchTerm", $"%{search}%");
			}

			if (timeframe != DateRangeEnum.None)
			{
				query.SetDateTime("minDate", minDate)
					.SetDateTime("maxDate", maxDate);
			}

			return query.DynamicList();
		}

		public IList<OpenPOModel> GetOpenProductPORequests(int ProductId)
		{
			var queryText = $@"Select p.PurchaseOrderID, p.ExpectedDate,  SUM(Quantity) POs, d.RecievedPOs
								From PurchaseOrder p
								Inner Join PurchaseOrderProduct pop
								ON pop.PurchaseOrderID = p.PurchaseOrderID
								LEFT JOIN
								(SELECT pop2.PurchaseOrderProductID, SUM(QuantityReceived) RecievedPOs FROM PurchaseOrderProductReceipt popr
								INNER JOIN PurchaseOrderProduct pop2 on pop2.PurchaseOrderProductID = popr.PurchaseOrderProductID
								GROUP BY pop2.PurchaseOrderProductID) d ON d.PurchaseOrderProductID = pop.PurchaseOrderProductID
								Where pop.ProductID = @productID AND p.IsDoNotReceive = 0
								GROUP BY p.PurchaseOrderID, d.RecievedPOs, p.ExpectedDate
								Order by ExpectedDate DESC
			";



			using (System.Data.SqlClient.SqlConnection client = new System.Data.SqlClient.SqlConnection(_connectionStrings.DefaultContext))
			{
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

				client.Open();
				cmd.Connection = client;
				cmd.CommandText = queryText;
				cmd.Parameters.AddWithValue("@productID", ProductId);

				using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
				{

					List<OpenPOModel> list = new List<OpenPOModel>();
					while (reader.Read())
					{
						OpenPOModel po = new OpenPOModel()
						{
							PurchaseOrderID = reader["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(reader["PurchaseOrderID"]) : 0,
							POs = reader["POs"] != DBNull.Value ? Convert.ToInt32(reader["POs"]) : 0,
							RecievedPOs = reader["RecievedPOs"] != DBNull.Value ? Convert.ToInt32(reader["RecievedPOs"]) : 0,
							ExpectedDate = reader["ExpectedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpectedDate"]) : DateTime.MinValue
						};

						list.Add(po);
					}

					return list;
				}
			}

		}

		public IList<dynamic> GetHistoryForProduct(int productID)
		{
			var query = @"
				SELECT
					po.SubmitDate,
					pop.Quantity,
					pop.UnitOfMeasureCodeID,
					pop.Price,
					po.VendorID,
					v.VendorName
				FROM PurchaseOrderProduct pop
				JOIN PurchaseOrder po on po.PurchaseOrderID = pop.PurchaseOrderID
				LEFT JOIN Vendor v ON v.VendorID = po.VendorID
				WHERE ProductID = :productID AND po.SubmitDate IS NOT NULL
			";

			return _dbContext.Session
				.CreateSQLQuery(query)
				.SetInt32("productID", productID)
				.DynamicList();
		}

		public PurchaseOrder GetPurchaseOrder(int purchaseOrderID) {
			return _dbContext.Get<PurchaseOrder>(purchaseOrderID);
		}

		public PurchaseOrder GetPurchaseOrder(int purchaseOrderID, bool showAll, bool showInternational, bool showDomesticDistributors, bool showDomesticNonDistributors)
		{
			var purchaseOrder = _dbContext.Get<PurchaseOrder>(purchaseOrderID);
			var authorized = true;
			if (!showAll)
			{
				authorized = false;
				var vendor = _dbContext.Get<Vendor>(purchaseOrder.VendorID);
				if (showInternational && vendor.IsInternational)
				{
					authorized = true;
				}
				if (showDomesticDistributors && vendor.IsDomesticDistributor)
				{
					authorized = true;
				}
				if (showDomesticNonDistributors && vendor.IsDomestic)
				{
					authorized = true;
				}
			}
			return authorized ? purchaseOrder : null;
		}

		public Task<IList<dynamic>> GetPurchaseOrderReceiptProducts(int purchaseOrderID)
		{
			var queryText = $@"
				SELECT
					popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductReceiptID)}
					,pop.{nameof(PurchaseOrderProduct.ProductID)}
					,pop.{nameof(PurchaseOrderProduct.PurchaseOrderProductID)}
					,p.{nameof(Product.ProductCustomID)}
					,p.{nameof(Product.ProductName)}
					,Code.{nameof(Code.CodeDescription)} UnitOfMeasure
					,pop.{nameof(PurchaseOrderProduct.Quantity)} OrderQuantity
					,_qtyReceived.{nameof(PurchaseOrderProductReceipt.QuantityReceived)} QuantityAlreadyReceived
					,popr.{nameof(PurchaseOrderProductReceipt.QuantityReceived)}
					,popr.{nameof(PurchaseOrderProductReceipt.SerialNumbers)}
				FROM PurchaseOrderProduct pop
					LEFT JOIN Product p ON p.{nameof(Product.ProductID)} = pop.{nameof(PurchaseOrderProduct.ProductID)}
					LEFT JOIN Code ON Code.{nameof(Code.CodeID)} = pop.{nameof(Product.UnitOfMeasureCodeID)}
					LEFT JOIN (
						SELECT
							{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
							,SUM({nameof(PurchaseOrderProductReceipt.QuantityReceived)}) {nameof(PurchaseOrderProductReceipt.QuantityReceived)}
						FROM PurchaseOrderProductReceipt popr
						GROUP BY PurchaseOrderProductID
					) _qtyReceived ON _qtyReceived.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)} = pop.{nameof(PurchaseOrderProduct.PurchaseOrderProductID)}
					LEFT JOIN (
						SELECT
							MAX(popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductReceiptID)}) {nameof(PurchaseOrderProductReceipt.PurchaseOrderProductReceiptID)}
							,popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)} {nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
						FROM PurchaseOrderProductReceipt popr
						GROUP BY popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
					) maxpopr ON maxpopr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)} = pop.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)}
					LEFT JOIN PurchaseOrderProductReceipt popr ON popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductReceiptID)} = maxpopr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductReceiptID)}
				WHERE pop.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = :purchaseOrderID
			";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("purchaseOrderID", purchaseOrderID)
				.DynamicListAsync();
		}

		public IEnumerable<dynamic> GetPurchaseOrderProducts(int purchaseOrderID)
		{
			var queryText = $@"
				SELECT
					pop.{nameof(PurchaseOrderProduct.PurchaseOrderProductID)}
					,pop.{nameof(PurchaseOrderProduct.PurchaseOrderID)}
					,pop.{nameof(PurchaseOrderProduct.ProductID)}
					,p.{nameof(Product.ProductName)}
					,pop.{nameof(PurchaseOrderProduct.UnitOfMeasureCodeID)}
					,pop.{nameof(PurchaseOrderProduct.Quantity)}
					,pop.{nameof(PurchaseOrderProduct.Price)}
				FROM PurchaseOrderProduct pop
					LEFT JOIN Product p ON p.{nameof(Product.ProductID)} = pop.{nameof(PurchaseOrderProduct.ProductID)}
				WHERE pop.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = :purchaseOrderID
			";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("purchaseOrderID", purchaseOrderID)
				.DynamicList();
		}

		public IEnumerable<dynamic> GetPurchaseOrderReportProducts(int purchaseOrderID)
		{
			var queryText = $@"
				SELECT
					pop.{nameof(PurchaseOrderProduct.ProductID)}
					,p.{nameof(Product.ProductCustomID)}
					,p.{nameof(Product.ProductName)}
					,(
						SELECT ISNULL(SUM(popr.QuantityReceived), 0)
						FROM PurchaseOrderProductReceipt popr
						WHERE popr.{nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID)} = pop.{nameof(PurchaseOrderProduct.PurchaseOrderProductID)}
					) QuantityPacked
					,pop.{nameof(PurchaseOrderProduct.Quantity)}
					,pop.{nameof(PurchaseOrderProduct.Price)}
				FROM PurchaseOrderProduct pop
					LEFT JOIN Product p ON p.{nameof(Product.ProductID)} = pop.{nameof(PurchaseOrderProduct.ProductID)}
				WHERE pop.{nameof(PurchaseOrderProduct.PurchaseOrderID)} = :purchaseOrderID
			";

			return _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("purchaseOrderID", purchaseOrderID)
				.DynamicList();
		}

		public PurchaseOrderReceiveInfo GetPurchaseOrderReceiveInfo(int purchaseOrderID)
		{
			var queryText = $@"
				SELECT
					po.{nameof(PurchaseOrder.PurchaseOrderID)},
					po.{nameof(PurchaseOrder.PurchaseOrderCustomID)},
					po.{nameof(PurchaseOrder.SubmitDate)},
					po.{nameof(PurchaseOrder.ApprovalDate)},
					po.{nameof(PurchaseOrder.ApprovedBy)},
					po.{nameof(PurchaseOrder.VendorID)},
					po.{nameof(PurchaseOrder.VendorOrderNumber)},
					vn.{nameof(Vendor.City)} FromCity,
					shipFromState.{nameof(Code.CodeName)} FromState,
					shipFromCountry.{nameof(Code.CodeDescription)} FromCountry,
					po.{nameof(PurchaseOrder.ExpectedDate)},
					po.{nameof(PurchaseOrder.ShipCompanyType)} ShipCompany,
					shipMethod.{nameof(Code.CodeDescription)} ShipMethod,
					po.{nameof(PurchaseOrder.IsPartialShipAcceptable)},
					po.{nameof(PurchaseOrder.ShippingCharge)},
					po.{nameof(PurchaseOrder.Notes)},
					po.{nameof(PurchaseOrder.UpdatedBy)},
					po.{nameof(PurchaseOrder.UpdatedOn)}
				FROM PurchaseOrder po
					LEFT JOIN Vendor vn ON po.VendorID = vn.VendorID
					LEFT JOIN Code shipMethod ON shipMethod.{nameof(Code.CodeID)} = po.{nameof(PurchaseOrder.ShipChoiceCodeID)}
					LEFT JOIN Code shipFromState ON shipFromState.{nameof(Code.CodeID)} = vn.{nameof(Vendor.StateCodeID)}
					LEFT JOIN Code shipFromCountry ON shipFromCountry.{nameof(Code.CodeID)} = vn.{nameof(Vendor.CountryCodeID)}
				WHERE po.{nameof(PurchaseOrder.PurchaseOrderID)} = :purchaseOrderID
			";

			var pori = _dbContext.Session
				.CreateSQLQuery(queryText)
				.SetInt32("purchaseOrderID", purchaseOrderID)
				.Dynamic();

			return new PurchaseOrderReceiveInfo(pori);
		}

		public PurchaseOrder SavePurchaseOrder(PurchaseOrder purchaseOrder)
		{
			_dbContext.BeginTransaction();
			_dbContext.Save(purchaseOrder);
			_dbContext.Commit();

			return purchaseOrder;
		}

		public bool UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
		{
			_dbContext.BeginTransaction();
			_dbContext.SaveOrUpdate(purchaseOrder);
			_dbContext.Commit();
			return true;
		}

		public bool DeletePurchaseOrder(PurchaseOrder purchaseOrder)
		{
			_dbContext.BeginTransaction();
			_dbContext.Delete<PurchaseOrder>(purchaseOrder);
			_dbContext.Commit();
			return true;
		}

		public Task SavePurchaseOrderProducts(List<PurchaseOrderProduct> purchaseOrderProducts)
		{
			if(!purchaseOrderProducts.Any()) {
				return null;
			}

			var properties = typeof(PurchaseOrderProduct).GetProperties().Select(p => p.Name);
			var tempTableName = $"TempPurchaseOrderProduct{Guid.NewGuid().ToString().Replace("-", "")}";
			var queryText = $@"
				DROP TABLE IF EXISTS #{tempTableName}
				SELECT * INTO #{tempTableName} FROM PurchaseOrderProduct where 1 = 0
				SET IDENTITY_INSERT #{tempTableName} ON
				INSERT INTO #{tempTableName} ({string.Join(',', properties)}) VALUES
			";

			for(var i = 0; i < purchaseOrderProducts.Count; i++) {
				if (i > 0) { queryText += ","; }

				var props = properties.Select(x => $":{x}{i}");
				queryText += $"({string.Join(',', props)})";
			}

			queryText += $@"
				MERGE PurchaseOrderProduct as t
					USING(SELECT * FROM #{tempTableName}) as s
						ON s.PurchaseOrderProductID = t.PurchaseOrderProductID
					WHEN MATCHED THEN
						UPDATE SET
							  ProductID           = s.ProductID
							, UnitOfMeasureCodeID = s.UnitOfMeasureCodeID
							, Quantity            = s.Quantity
							, Price               = s.Price
					WHEN NOT MATCHED BY TARGET THEN
						INSERT VALUES (
							 s.PurchaseOrderID
							,s.ProductID
							,s.UnitOfMeasureCodeID
							,s.Quantity
							,s.Price
						)
				;
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			for(var i = 0; i < purchaseOrderProducts.Count; i++) {
				query.SetInt32($"PurchaseOrderProductID{i}", purchaseOrderProducts[i].PurchaseOrderProductID);
				query.SetInt32($"PurchaseOrderID{i}", purchaseOrderProducts[i].PurchaseOrderID);
				query.SetInt32($"ProductID{i}", purchaseOrderProducts[i].ProductID);
				query.SetInt32($"UnitOfMeasureCodeID{i}", purchaseOrderProducts[i].UnitOfMeasureCodeID);
				query.SetInt32($"Quantity{i}", purchaseOrderProducts[i].Quantity);
				query.SetDecimal($"Price{i}", purchaseOrderProducts[i].Price);
			}

			return query.ExecuteUpdateAsync();
		}

		public int SavePurchaseOrderReceipt(List<PurchaseOrderProductReceipt> purchaseOrderProductReceipts)
		{
			var properties = new List<string>(){
				nameof(PurchaseOrderProductReceipt.PurchaseOrderProductID),
				nameof(PurchaseOrderProductReceipt.QuantityReceived),
				nameof(PurchaseOrderProductReceipt.SerialNumbers),
				nameof(PurchaseOrderProductReceipt.ReceiptDate)
			};

			var queryText = $@"
				INSERT INTO PurchaseOrderProductReceipt ({string.Join(',', properties)})
					VALUES
			";

			var modelList = purchaseOrderProductReceipts.ToList();
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
				query.SetInt32($"PurchaseOrderProductID{i}", model.PurchaseOrderProductID);
				query.SetInt32($"QuantityReceived{i}", model.QuantityReceived);
				query.SetString($"SerialNumbers{i}", model.SerialNumbers);
				query.SetParameter<DateTime?>($"ReceiptDate{i}", model.ReceiptDate);
			}

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();

			return results;
		}

		public bool DeletePurchaseOrderProduct(List<int> purchaseOrderProductIDs)
		{
			var idStrings = purchaseOrderProductIDs.Select(x => $":popID{x}");
			var queryText = $@"
			DELETE pop FROM PurchaseOrderProduct pop
			JOIN PurchaseOrder po ON po.PurchaseOrderID = pop.PurchaseOrderID
			WHERE
				pop.PurchaseOrderProductID IN ({string.Join(',', idStrings)})
				AND po.ApprovalDate IS NULL
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);

			foreach(var id in purchaseOrderProductIDs)
				query.SetInt32($"popID{id}", id);

			var deleteCount = query.ExecuteUpdate();

			return deleteCount > 0;
		}

		public bool ApprovePurchaseOrder(int purchaseOrderID, int userID)
		{
			var queryText = @"
				UPDATE PurchaseOrder SET
					ApprovalDate = GETUTCDATE(),
					ApprovedBy = :userID
				WHERE
					PurchaseOrderID = :purchaseOrderID
					AND ApprovalDate IS NULL
			";

			var updateCount = _dbContext.Session.CreateSQLQuery(queryText)
				.SetInt32("userID", userID)
				.SetInt32("purchaseOrderID", purchaseOrderID)
				.ExecuteUpdate();

			return updateCount == 1;
		}

		public IList<dynamic> GetPeachTreeReceiptsContents()
		{
			return _dbContext.Session
				.CreateSQLQuery("EXEC GeneratePurchaseOrderPeachTreeInvoice")
				.DynamicList();
		}

		public DailyPurchaseOrderCount GetDailyPurchaseOrderCount(int vendorID)
		{
			return _dbContext.DailyPurchaseOrderCounts.FirstOrDefault(x => x.VendorID == vendorID);
		}

		public DailyPurchaseOrderCount SaveDailyPurchaseOrderCount(int vendorID, DateTime now, int count)
		{
			var model = new DailyPurchaseOrderCount { VendorID = vendorID, LastCreated = now, DailyCount = count };
			_dbContext.BeginTransaction();
			_dbContext.Save(model);
			_dbContext.Commit();

			return model;
		}

		public void UpdateDailyPurchaseOrderCount(DailyPurchaseOrderCount count)
		{
			_dbContext.BeginTransaction();
			_dbContext.Update(count);
			_dbContext.Commit();
		}
	}
}
