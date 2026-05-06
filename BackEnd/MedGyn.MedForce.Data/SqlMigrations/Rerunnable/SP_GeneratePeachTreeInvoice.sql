CREATE OR ALTER   PROCEDURE [dbo].[GeneratePeachTreeInvoice]
AS BEGIN
       insert into AccountingExportBatch (ExportTS) values (getdate())
       declare @newBatchID int = @@IDENTITY
       update customerordershipment set AccountingExportBatchID=@newBatchID where AccountingExportBatchID is null and InvoiceSent=1
       --declare @newBatchID int = 60--(select max(AccountingExportBatchID) from AccountingExportBatch)


	declare @temp table ([Customer ID] varchar(max), [Invoice/CM #] varchar(100), [Date] varchar(max)
						, [Ship to Name] varchar(max), [Ship to Address-Line One] varchar(max), [Ship to Address-Line Two] varchar(max)
						, [Ship to City] varchar(max), [Ship to State] varchar(max), [Ship to Zipcode] varchar(max), [Ship to Country] varchar(max)
						, [Customer PO] varchar(max), [Ship Via] varchar(max), [Ship Date] varchar(max), [Date Due] varchar(max), [Displayed Terms] varchar(max)
						, [Sales Representative ID] varchar(max), [Accounts Receivable Account] varchar(max)
						, [Invoice Note] varchar(max), [Note Prints After Line Items] varchar(max) 
						, [Number of Distributions] varchar(max), [Invoice/CM Distribution] varchar(100)
						, [Quantity] int, [Item ID] varchar(max), [Serial Number] varchar(max), [Description] varchar(max), [G/L Account] varchar(max)
						, [Unit Price] money, [Tax Type] int, [Amount] money, [U/M ID] varchar(max), [U/M No. of Stocking Units] int
						, [Sales Tax ID] varchar(100), [Sales Tax Agency ID] varchar(100))

	declare @output table (sortseq varchar(100), bigfield varchar(max))



	--grab all the products on the order
	insert into @temp
	select  
			c.CustomerCustomID as [Customer ID]
	--, co.CustomerOrderCustomID as [Invoice/CM #] 
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
	, (select count(*)+1 from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID and f2.CustomerOrderProductFillID<f.CustomerOrderProductFillID) as [Invoice/CM Distribution]
	, f.quantitypacked as [Quantity]
	, convert(varchar, p.productcustomid) as [Item ID]
	, f.serialnumbers as [Serial Number]
	, replace(p.productname, ',',' ') as [Description]
	, debit.codename as [G/L Account]
	, convert(varchar,cop.price) as [Unit Price]
	, case when salestaxcodes.CodeDescription='exempt' then 2 else 1 end as [Tax Type]
	, -1 * cop.quantity * cop.price as [Amount]
	, '<Each>' as [U/M ID]
	, 1 as [U/M No. of Stocking Units]
	, '' as [Sales Tax ID] 
	, '' as [Sales Tax Agency ID]

	from customer c
	left join code salestaxcodes on salestaxcodes.codeid=c.salestaxcodeid

	left join code debit on debit.codeid=c.GLSalesCodeID --if order or if invoiced
	left join code credit on credit.codeid=c.GLAccountsReceivableCodeID --invoiced

	inner join customerorder co on co.customerid=c.customerid
	inner join customerordershipment s on s.customerorderid=co.customerorderid
	inner join customerordershipmentbox b on b.customerordershipmentid=s.customerordershipmentid
	inner join customerorderproductfill f on f.customerordershipmentboxid=b.customerordershipmentboxid

	inner join code shipper on shipper.codeid=s.shipcompanytype

	inner join customershippinginfo loc on loc.customershippinginfoid=co.customershippinginfoid
	left join code statecodes on statecodes.codeid=loc.statecodeid
	inner join code countrycodes on countrycodes.codeid=loc.countrycodeid
	inner join [user] u on u.userid=loc.repuserid

	inner join customerorderproduct cop on cop.CustomerOrderProductID=f.CustomerOrderProductID
	inner join product p on p.productid=cop.productid

	where s.accountingexportbatchid = @newBatchID





	--calculate the tax
	insert into @temp
	select  
			c.CustomerCustomID as [Customer ID]
	--, co.CustomerOrderCustomID as [Invoice/CM #] 
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
	, (select count(*)+1 from CustomerOrderShipmentBox b2 inner join CustomerOrderProductFill f2 on f2.CustomerOrderShipmentBoxID=b2.CustomerOrderShipmentBoxID where b2.CustomerOrderShipmentID=b.CustomerOrderShipmentID ) as [Invoice/CM Distribution]
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
	--inner join customerorderproductfill f on f.customerordershipmentboxid=b.customerordershipmentboxid

	inner join code shipper on shipper.codeid=s.shipcompanytype

	inner join customershippinginfo loc on loc.customershippinginfoid=co.customershippinginfoid
	left join code statecodes on statecodes.codeid=loc.statecodeid
	inner join code countrycodes on countrycodes.codeid=loc.countrycodeid
	inner join [user] u on u.userid=loc.repuserid

	--inner join customerorderproduct cop on cop.CustomerOrderProductID=f.CustomerOrderProductID
	--inner join product p on p.productid=cop.productid

	where s.accountingexportbatchid = @newBatchID




	--grab the shipping on the order
	insert into @temp
	select  
			c.CustomerCustomID as [Customer ID]
	--, co.CustomerOrderCustomID as [Invoice/CM #] 
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
	--inner join customerorderproductfill f on f.customerordershipmentboxid=b.customerordershipmentboxid

	inner join code shipper on shipper.codeid=s.shipcompanytype

	inner join customershippinginfo loc on loc.customershippinginfoid=co.customershippinginfoid
	left join code statecodes on statecodes.codeid=loc.statecodeid
	inner join code countrycodes on countrycodes.codeid=loc.countrycodeid
	inner join [user] u on u.userid=loc.repuserid

	--inner join customerorderproduct cop on cop.CustomerOrderProductID=f.CustomerOrderProductID
	--inner join product p on p.productid=cop.productid

	where s.accountingexportbatchid = @newBatchID











	--build the file
	insert into @output
		select '0'
				, 'Customer ID,Invoice/CM #,Date,Ship to Name,Ship to Address-Line One,Ship to Address-Line Two,Ship to City,Ship to State,Ship to Zipcode'
				+ ',Ship to Country,Customer PO,Ship Via,Ship Date,Date Due,Displayed Terms,Sales Representative ID,Accounts Receivable Account'
				+ ',Invoice Note,Note Prints After Line Items,Number of Distributions,Invoice/CM Distribution,Quantity,Item ID,Serial Number'
				+ ',Description,G/L Account,Unit Price,Amount,U/M ID,U/M No. of Stocking Units,Tax Type,Sales Tax ID,Sales Tax Agency ID'

	insert into @output
		select RIGHT('00000000000000000000'+[Invoice/CM #],20) + '-' + RIGHT(case when [Invoice/CM Distribution]=0 then '999999' else '000000'+[Invoice/CM Distribution] end,6)
			,	concat([Customer ID]
					, ',' ,[Invoice/CM #]
					, ',' ,[Date]
					, ',' ,[Ship to Name]
					, ',' ,[Ship to Address-Line One]
					, ',' ,[Ship to Address-Line Two]
					, ',' ,[Ship to City]
					, ',' ,[Ship to State]
					, ',' ,[Ship to Zipcode]
					, ',' ,[Ship to Country]
					, ',' ,[Customer PO]
					, ',' ,[Ship Via]
					, ',' ,[Ship Date]
					, ',' ,[Date Due]
					, ',' ,[Displayed Terms]
					, ',' ,[Sales Representative ID]
					, ',' ,[Accounts Receivable Account]
					, ',' ,[Invoice Note]
					, ',' ,[Note Prints After Line Items]
					, ',' ,[Number of Distributions]
					, ',' ,[Invoice/CM Distribution]
					, ',' ,[Quantity]
					, ',' ,[Item ID]
					, ',' ,[Serial Number]
					, ',' ,[Description]
					, ',' ,[G/L Account]
					, ',' ,[Unit Price]
					, ',' ,[Amount]
					, ',' ,[U/M ID]
					, ',' ,[U/M No. of Stocking Units]
					, ',' ,[Tax Type]
					, ',' ,[Sales Tax ID] 
					, ',' ,[Sales Tax Agency ID] 
				)
		from @temp

	--insert into @output
	--	select 'zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz', ''

	select bigfield as [content] from @output order by sortseq

end

