


/* ======= This is 202006091529_MigrateZipFromIntToString.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

-- Converts INT col to string (sql server will preserve the value)

ALTER TABLE Customer
	ALTER COLUMN ZipCode VARCHAR(50) NOT NULL

ALTER TABLE Customer
	ALTER COLUMN BankZipCode VARCHAR(50) NULL


ALTER TABLE CustomerShippingInfo
	ALTER COLUMN ZipCode VARCHAR(50) NULL


ALTER TABLE Vendor
	ALTER COLUMN ZipCode VARCHAR(50) NOT NULL

ALTER TABLE Vendor
	ALTER COLUMN BankZipCode VARCHAR(50) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



