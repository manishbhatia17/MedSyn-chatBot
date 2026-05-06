
/* ======= This is 202005131557_updateCustomerOrderShipment.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

IF COL_LENGTH('CustomerOrderShipment', 'ShippingCharge') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD ShippingCharge DECIMAL(10,2) NULL

IF COL_LENGTH('CustomerOrderShipment', 'CreatedBy') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD CreatedBy INT NULL

IF COL_LENGTH('CustomerOrderShipment', 'CreatedOn') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD CreatedOn DATETIME2 NULL

ALTER TABLE CustomerOrderShipment ALTER COLUMN ShipCompanyType INT NULL
ALTER TABLE CustomerOrderShipment ALTER COLUMN ShipMethodCodeID INT NULL
ALTER TABLE CustomerOrderShipment ALTER COLUMN ShipAccountNumber VARCHAR(100) NULL

-- Have to run this in an EXEC in order for the column to exist
EXEC('
UPDATE CustomerOrderShipment SET
	CreatedBy = (SELECT TOP 1 UserID FROM [User])
	,CreatedOn = GETUTCDATE()
')

EXEC('
ALTER TABLE CustomerOrderShipment ALTER COLUMN CreatedBy INT NOT NULL
ALTER TABLE CustomerOrderShipment ALTER COLUMN CreatedOn DATETIME2 NOT NULL
')




/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



