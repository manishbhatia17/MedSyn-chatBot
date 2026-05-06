CREATE OR ALTER   PROCEDURE [dbo].[GeneratePurchaseOrderPeachTreeInvoice]
AS BEGIN
	--take it out of received and put in accounts payable

	insert into AccountingExportBatch (ExportTS) values (getdate())
	declare @newBatchID int = @@IDENTITY
	update purchaseorderproductreceipt set AccountingExportBatchID=@newBatchID where AccountingExportBatchID is null 
	--declare @newBatchID int = (select max(AccountingExportBatchID) from purchaseorderproductreceipt)

	--grab all the products on the order
	select 
			v.vendorcustomid as [Vendor ID]
			,po.purchaseordercustomid as [PO #]
			,format(po.submitdate, 'MM-dd-yyyy') as [Date]
			,ap.codename as [Accounts Payable Account]
			,1 as [Number of Distributions]
			,1 as [PO Distribution]
			,r.quantityreceived as [Quantity]
			,p.productcustomid as [Item ID]
			,'<Each>' as [U/M ID]
			,gl.codename as [G/L Account]
			,pol.price as [Unit Price]
			,r.quantityreceived * pol.price as [Amount]
			,1 as [U/M No. of Stocking Units]
			,p.productname as [Description]
	into #temp
	from purchaseorderproductreceipt r
	inner join purchaseorderproduct pol on pol.purchaseorderproductid=r.purchaseorderproductid
	inner join product p on p.productid=pol.productid
	inner join purchaseorder po on po.purchaseorderid=pol.purchaseorderid
	inner join vendor v on v.vendorid=po.vendorid
	left join code ap on ap.codeid=v.GLAccountsPayableCodeID
	left join code gl on gl.codeid=v.GLPurchaseCodeID
	where r.accountingexportbatchid = @newBatchID
	order by r.purchaseorderproductreceiptid


	select STRING_AGG(name, ',') as content
	from tempdb.sys.columns where object_id=object_id('tempdb..#temp')
	union all
	select concat(
			[Vendor ID]
			, ',' , [PO #]
			, ',' , [Date]
			, ',' , [Accounts Payable Account]
			, ',' , [Number of Distributions]
			, ',' , [PO Distribution]
			, ',' , [Quantity]
			, ',' , [Item ID]
			, ',' , [U/M ID]
			, ',' , [G/L Account]
			, ',' , [Unit Price]
			, ',' , [Amount]
			, ',' , [U/M No. of Stocking Units]
			, ',' , [Description]
			)
	from #temp
	--union all
	--select ''

	drop table #temp

END