


/* ======= This is 202005201704_stuffForFedex.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


DELETE FROM Code WHERE CodeTypeID = 11

IF COL_LENGTH('CodeType', 'LockCodes') IS NULL
	ALTER TABLE CodeType
	ADD LockCodes BIT NOT NULL
		CONSTRAINT D_CodeType_LockCodes DEFAULT(0)

IF COL_LENGTH('CustomerOrderShipment', 'InvoiceNumber') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD InvoiceNumber VARCHAR(100) NULL

IF COL_LENGTH('CustomerOrderShipment', 'InvoiceDate') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD InvoiceDate DATETIME2 NULL

IF COL_LENGTH('CustomerOrderShipment', 'MasterTrackingNumber') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD MasterTrackingNumber VARCHAR(100) NULL

IF COL_LENGTH('CustomerOrderShipment', 'InvoiceSent') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD InvoiceSent BIT NOT NULL
		CONSTRAINT D_CodeType_InvoiceSent DEFAULT(0)

IF COL_LENGTH('CustomerOrderShipment', 'PeachTreeExportBatchID') IS NULL
	ALTER TABLE CustomerOrderShipment
	ADD PeachTreeExportBatchID INT NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



