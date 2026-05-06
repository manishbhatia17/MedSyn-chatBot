


/* ======= This is 202005122210_updateCustomerShippingInfo.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

ALTER TABLE CustomerShippingInfo DROP COLUMN IF EXISTS FedexAccountID
ALTER TABLE CustomerShippingInfo DROP COLUMN IF EXISTS UPSAccountID
ALTER TABLE CustomerShippingInfo DROP COLUMN IF EXISTS USMailAccountID
ALTER TABLE CustomerShippingInfo DROP COLUMN IF EXISTS DHLAccountID

IF COL_LENGTH('CustomerShippingInfo', 'ShipCompany1CodeID') IS NULL
	ALTER TABLE CustomerShippingInfo
	ADD ShipCompany1CodeID INT NULL

IF COL_LENGTH('CustomerShippingInfo', 'ShipCompany1AccountNumber') IS NULL
	ALTER TABLE CustomerShippingInfo
	ADD ShipCompany1AccountNumber VARCHAR(100) NULL

IF COL_LENGTH('CustomerShippingInfo', 'ShipCompany2CodeID') IS NULL
	ALTER TABLE CustomerShippingInfo
	ADD ShipCompany2CodeID INT NULL

IF COL_LENGTH('CustomerShippingInfo', 'ShipCompany2AccountNumber') IS NULL
	ALTER TABLE CustomerShippingInfo
	ADD ShipCompany2AccountNumber VARCHAR(100) NULL


DELETE FROM CodeType WHERE CodeTypeID in (14)
DELETE FROM Code WHERE CodeTypeID in (14)

UPDATE CodeType SET CodeTypeName = 'FedExShipMethods' WHERE CodeTypeID = 11
UPDATE CodeType SET CodeTypeName = 'UPSShipMethods' WHERE CodeTypeID = 12
UPDATE CodeType SET CodeTypeName = 'OtherShipMethods' WHERE CodeTypeID = 13





/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



