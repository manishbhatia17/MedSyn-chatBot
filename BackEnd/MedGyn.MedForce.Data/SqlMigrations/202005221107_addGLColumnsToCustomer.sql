
/* ======= This is 202005221107_addGLColumnsToCustomer.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

---------------------- Domestic Columns---------------------
IF COL_LENGTH('Customer', 'DomesticGLSalesCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticGLSalesCodeID INT NULL

IF COL_LENGTH('Customer', 'DomesticGLShippingChargeCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticGLShippingChargeCodeID INT NULL

IF COL_LENGTH('Customer', 'DomesticGLAccountsReceivableCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticGLAccountsReceivableCodeID INT NULL

---------------------- DomesticAfaxys Columns---------------------
IF COL_LENGTH('Customer', 'DomesticAfaxysGLSalesCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticAfaxysGLSalesCodeID INT NULL

IF COL_LENGTH('Customer', 'DomesticAfaxysGLShippingChargeCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticAfaxysGLShippingChargeCodeID INT NULL

IF COL_LENGTH('Customer', 'DomesticAfaxysGLAccountsReceivableCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticAfaxysGLAccountsReceivableCodeID INT NULL

---------------------- DomesticDistributor Columns---------------------
IF COL_LENGTH('Customer', 'DomesticDistributorGLSalesCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticDistributorGLSalesCodeID INT NULL

IF COL_LENGTH('Customer', 'DomesticDistributorGLShippingChargeCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticDistributorGLShippingChargeCodeID INT NULL

IF COL_LENGTH('Customer', 'DomesticDistributorGLAccountsReceivableCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD DomesticDistributorGLAccountsReceivableCodeID INT NULL

---------------------- International Columns---------------------
IF COL_LENGTH('Customer', 'InternationalGLSalesCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD InternationalGLSalesCodeID INT NULL

IF COL_LENGTH('Customer', 'InternationalGLShippingChargeCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD InternationalGLShippingChargeCodeID INT NULL

IF COL_LENGTH('Customer', 'InternationalGLAccountsReceivableCodeID') IS NULL
	ALTER TABLE dbo.Customer
		ADD InternationalGLAccountsReceivableCodeID INT NULL



------------------- Drop Deprecated GL Column--------------------
ALTER TABLE dbo.Customer	DROP COLUMN IF EXISTS [GLAccountNumberCodeID];



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



