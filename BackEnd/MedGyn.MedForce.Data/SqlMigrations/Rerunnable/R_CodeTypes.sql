DROP TABLE IF EXISTS #TempCodeType
CREATE TABLE #TempCodeType (
	CodeTypeID INT NOT NULL,
	CodeTypeName VARCHAR(100) NOT NULL,
	IsDeleted BIT NOT NULL,
	LockCodes BIT NOT NULL,
)

INSERT INTO #TempCodeType VALUES
(1, 'States', 0, 0)
,(2, 'Countries', 0, 0)
,(3, 'VendorGL', 1, 0)
,(4, 'U/M', 0, 0)
,(5, 'ShipWeightUnit', 0, 1)
,(6, 'ShipDimensionsUnit', 0, 1)
,(7, 'CustomerGL', 1, 0)
,(8, 'CustomerPracticeType', 0, 0)
,(9, 'CustomerSalesTax', 0, 0)
,(10, 'CustomerStatus', 0, 0)
,(11, 'FedExShipMethod', 0, 1)
,(12, 'UPSShipMethod', 0, 1)
,(13, 'OtherShipMethod', 0, 0)
-- 14 removed
,(15, 'InventoryAdjustmentReason', 0, 0)
,(16, 'ShipCompanies', 0, 0)
,(17, 'VendorPurchasesGL', 0, 0)
,(18, 'VendorFreightChargesGL', 0, 0)
,(19, 'VendorAccountsPayableGL', 0, 0)
,(20, 'CustomerSalesGL', 0, 0)
,(21, 'CustomerShippingChargeGL', 0, 0)
,(22, 'CustomerAccountsReceivableGL', 0, 0)
,(23, 'PaymentType', 0, 0)
,(24, 'VendorStatus', 0, 0)
,(25, 'UPSFreightShipMethod', 0, 1)

MERGE CodeType as target
	USING(SELECT * FROM #TempCodeType) as source
		ON source.CodeTypeID = target.CodeTypeID
	WHEN MATCHED THEN
		UPDATE SET
			 CodeTypeName = source.CodeTypeName
			,IsDeleted    = source.IsDeleted
			,LockCodes    = source.LockCodes
	WHEN NOT MATCHED BY TARGET THEN
		INSERT VALUES (
			source.CodeTypeID
			,source.CodeTypeName
			,source.IsDeleted
			,source.LockCodes
		);

DROP TABLE #TempCodeType
