


/* ======= This is 202005260950_RenameGLColumns.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

-- VENDOR GL COLUMNS

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Vendor'
						AND COLUMN_NAME = 'DomesticGLPurchaseCodeID' )
	exec sp_rename '[Vendor].DomesticGLPurchaseCodeID', 'GLPurchaseCodeID', 'COLUMN'

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Vendor'
						AND COLUMN_NAME = 'DomesticGLFreightChargeCodeID' )
	exec sp_rename '[Vendor].DomesticGLFreightChargeCodeID', 'GLFreightChargeCodeID', 'COLUMN'

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Vendor'
						AND COLUMN_NAME = 'DomesticGLAccountsPayableCodeID' )
	exec sp_rename '[Vendor].DomesticGLAccountsPayableCodeID', 'GLAccountsPayableCodeID', 'COLUMN'

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [DomesticAfaxysGLPurchaseCodeID]

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [DomesticAfaxysGLFreightChargeCodeID]

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [DomesticAfaxysGLAccountsPayableCodeID]


ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [DomesticDistributorGLPurchaseCodeID]

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [DomesticDistributorGLFreightChargeCodeID]

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [DomesticDistributorGLAccountsPayableCodeID]


ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [InternationalGLPurchaseCodeID]

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [InternationalGLFreightChargeCodeID]

ALTER TABLE Vendor
	DROP COLUMN IF EXISTS [InternationalGLAccountsPayableCodeID]


-- CUSTOMER GL COLUMNS

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Customer'
						AND COLUMN_NAME = 'DomesticGLSalesCodeID' )
	exec sp_rename '[Customer].DomesticGLSalesCodeID', 'GLSalesCodeID', 'COLUMN'

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Customer'
						AND COLUMN_NAME = 'DomesticGLShippingChargeCodeID' )
	exec sp_rename '[Customer].DomesticGLShippingChargeCodeID', 'GLShippingChargeCodeID', 'COLUMN'

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Customer'
						AND COLUMN_NAME = 'DomesticGLAccountsReceivableCodeID' )
	exec sp_rename '[Customer].DomesticGLAccountsReceivableCodeID', 'GLAccountsReceivableCodeID', 'COLUMN'

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [DomesticAfaxysGLSalesCodeID]

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [DomesticAfaxysGLShippingChargeCodeID]

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [DomesticAfaxysGLAccountsReceivableCodeID]


ALTER TABLE Customer
	DROP COLUMN IF EXISTS [DomesticDistributorGLSalesCodeID]

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [DomesticDistributorGLShippingChargeCodeID]

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [DomesticDistributorGLAccountsReceivableCodeID]


ALTER TABLE Customer
	DROP COLUMN IF EXISTS [InternationalGLSalesCodeID]

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [InternationalGLShippingChargeCodeID]

ALTER TABLE Customer
	DROP COLUMN IF EXISTS [InternationalGLAccountsReceivableCodeID]


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



