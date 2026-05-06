


/* ======= This is 202005111119_COFillAndShipment.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

DROP TABLE IF EXISTS CustomerOrderShipment
CREATE TABLE CustomerOrderShipment (
	CustomerOrderShipmentID INT IDENTITY(1,1) NOT NULL,
	CustomerOrderID INT NOT NULL,
	ShipCompanyType INT NOT NULL,
	ShipMethodCodeID INT NOT NULL,
	ShipAccountNumber varchar(100) NOT NULL,
	FillOption INT NULL,
	NumberOfSameBoxes INT NULL,
	NumberOfPackingSlips INT NULL,
	ShipmentComplete BIT NOT NULL CONSTRAINT D_CustomerOrderShipment_ShipmentComplete DEFAULT(0),
	CONSTRAINT PK_CustomerOrderShipment PRIMARY KEY (CustomerOrderShipmentID)
)

DROP TABLE IF EXISTS CustomerOrderShipmentBox
CREATE TABLE CustomerOrderShipmentBox (
	CustomerOrderShipmentBoxID INT IDENTITY(1,1) NOT NULL,
	CustomerOrderShipmentID INT NULL,
	CustomerOrderID INT NOT NULL,
	Weight INT NULL,
	WeightUnitCodeID INT NULL,
	Length INT NULL,
	Width INT NULL,
	Depth INT NULL,
	DimensionUnitCodeID INT NULL,
	CONSTRAINT PK_CustomerOrderShipmentBox PRIMARY KEY (CustomerOrderShipmentBoxID)
)

IF COL_LENGTH('CustomerOrderProductFill', 'CustomerOrderShipmentBoxID') IS NULL
	ALTER TABLE CustomerOrderProductFill
	ADD CustomerOrderShipmentBoxID INT NULL

ALTER TABLE CustomerOrderProductFill
ALTER COLUMN SerialNumbers VARCHAR(MAX) NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



