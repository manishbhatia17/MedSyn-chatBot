


/* ======= This is 202005041404_CreateCustomerOrderTable.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

DROP TABLE IF EXISTS CustomerOrder, CustomerOrderLine, CustomerOrderLineFill
CREATE TABLE CustomerOrder (
	CustomerOrderID INT IDENTITY(1, 1) NOT NULL,
	CustomerOrderCustomID INT NULL,
	SubmitDate DATETIME2 NULL,
	MGApprovedOn DATETIME2 NULL,
	MGApprovedBy INT NULL,
	VPApprovedOn DATETIME2 NULL,
	VPApprovedBy INT NULL,
	CustomerID INT NOT NULL,
	CustomerShippingInfoID INT NOT NULL,
	PONumber VARCHAR(100) NULL,
	Contact VARCHAR(100) NULL,
	ShipCompanyType INT NULL,
	ShipChoiceCodeID INT NULL,
	IsPartialShipAcceptable BIT NOT NULL CONSTRAINT D_CustomerOrder_IsPartialShipAcceptable DEFAULT(0),
	IsDoNotFill BIT NOT NULL CONSTRAINT D_CustomerOrder_IsDoNotFill DEFAULT(0),
	ShippingCharge DECIMAL(10,2) NULL,
	HandlingCharge DECIMAL(10,2) NULL,
	InsuranceCharge DECIMAL(10,2) NULL,
	Instructions VARCHAR(MAX) NULL,
	Notes VARCHAR(MAX) NULL,
	UpdatedBy INT NOT NULL,
	UpdatedOn DATETIME2 NOT NULL,
	CONSTRAINT PK_CustomerOrder PRIMARY KEY (CustomerOrderID)
)

DROP TABLE IF EXISTS CustomerOrderProduct
CREATE TABLE CustomerOrderProduct (
	CustomerOrderProductID INT IDENTITY(1, 1) NOT NULL,
	CustomerOrderID INT NOT NULL,
	ProductID INT NOT NULL,
	UnitOfMeasureCodeID INT NOT NULL,
	Quantity INT NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	CONSTRAINT PK_CustomerOrderProduct PRIMARY KEY (CustomerOrderProductID)
)

DROP TABLE IF EXISTS CustomerOrderProductFill
CREATE TABLE CustomerOrderProductFill (
	CustomerOrderProductFillID INT IDENTITY(1, 1) NOT NULL,
	CustomerOrderProductID INT NOT NULL,
	QuantityPacked INT NOT NULL,
	SerialNumbers VARCHAR(200) NOT NULL,
	CONSTRAINT PK_CustomerOrderProductFill PRIMARY KEY (CustomerOrderProductFillID)
)




/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



