
/* ======= This is 202005211019_addGLColumnsToVendor.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

---------------------- Domestic Columns---------------------
IF COL_LENGTH('Vendor', 'IsDomestic') IS NULL
	ALTER TABLE dbo.Vendor
		ADD IsDomestic BIT NOT NULL
		CONSTRAINT D_Vendor_IsDomestic DEFAULT(0)

IF COL_LENGTH('Vendor', 'DomesticGLPurchaseCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticGLPurchaseCodeID INT NULL

IF COL_LENGTH('Vendor', 'DomesticGLFreightChargeCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticGLFreightChargeCodeID INT NULL

IF COL_LENGTH('Vendor', 'DomesticGLAccountsPayableCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticGLAccountsPayableCodeID INT NULL

---------------------- DomesticAfaxys Columns---------------------
IF COL_LENGTH('Vendor', 'IsDomesticAfaxys') IS NULL
	ALTER TABLE dbo.Vendor
		ADD IsDomesticAfaxys BIT NOT NULL
		CONSTRAINT D_Vendor_IsDomesticAfaxys DEFAULT(0)

IF COL_LENGTH('Vendor', 'DomesticAfaxysGLPurchaseCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticAfaxysGLPurchaseCodeID INT NULL

IF COL_LENGTH('Vendor', 'DomesticAfaxysGLFreightChargeCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticAfaxysGLFreightChargeCodeID INT NULL

IF COL_LENGTH('Vendor', 'DomesticAfaxysGLAccountsPayableCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticAfaxysGLAccountsPayableCodeID INT NULL

---------------------- DomesticDistributor Columns---------------------
IF COL_LENGTH('Vendor', 'IsDomesticDistributor') IS NULL
	ALTER TABLE dbo.Vendor
		ADD IsDomesticDistributor BIT NOT NULL
		CONSTRAINT D_Vendor_IsDomesticDistributor DEFAULT(0)

IF COL_LENGTH('Vendor', 'DomesticDistributorGLPurchaseCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticDistributorGLPurchaseCodeID INT NULL

IF COL_LENGTH('Vendor', 'DomesticDistributorGLFreightChargeCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticDistributorGLFreightChargeCodeID INT NULL

IF COL_LENGTH('Vendor', 'DomesticDistributorGLAccountsPayableCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD DomesticDistributorGLAccountsPayableCodeID INT NULL

---------------------- International Columns---------------------
IF COL_LENGTH('Vendor', 'IsInternational') IS NULL
	ALTER TABLE dbo.Vendor
		ADD IsInternational BIT NOT NULL
		CONSTRAINT D_Vendor_IsInternational DEFAULT(0)

IF COL_LENGTH('Vendor', 'InternationalGLPurchaseCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD InternationalGLPurchaseCodeID INT NULL

IF COL_LENGTH('Vendor', 'InternationalGLFreightChargeCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD InternationalGLFreightChargeCodeID INT NULL

IF COL_LENGTH('Vendor', 'InternationalGLAccountsPayableCodeID') IS NULL
	ALTER TABLE dbo.Vendor
		ADD InternationalGLAccountsPayableCodeID INT NULL



------------------- Drop Deprecated GL Column--------------------
ALTER TABLE dbo.Vendor	DROP COLUMN IF EXISTS [GLAccountNumberCodeID];



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



