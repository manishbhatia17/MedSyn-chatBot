


/* ======= This is 202004290913_CreatePurchaseOrderTables.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

DROP TABLE IF EXISTS PurchaseOrder, PurchaseOrderLine, PurchaseOrderLineReceipt
CREATE TABLE PurchaseOrder (
	PurchaseOrderID INT IDENTITY(1, 1) NOT NULL,
	PurchaseOrderCustomID INT NULL,
	SubmitDate DATETIME2 NULL,
	ApprovalDate DATETIME2 NULL,
	ApprovedBy INT NULL,
	VendorID INT NOT NULL,
	VendorOrderNumber VARCHAR(100) NULL,
	ExpectedDate DATETIME2 NULL,
	ShipCompanyType INT NULL,
	ShipChoiceCodeID INT NULL,
	IsPartialShipAcceptable BIT NOT NULL CONSTRAINT D_PurchaseOrder_IsPartialShipAcceptable DEFAULT(0),
	ShippingCharge DECIMAL(10,2) NULL,
	Notes VARCHAR(MAX) NULL,
	UpdatedBy INT NOT NULL,
	UpdatedOn DATETIME2 NOT NULL,
	CONSTRAINT PK_PurchaseOrder PRIMARY KEY (PurchaseOrderID)
)

DROP TABLE IF EXISTS PurchaseOrderProduct
CREATE TABLE PurchaseOrderProduct (
	PurchaseOrderProductID INT IDENTITY(1, 1) NOT NULL,
	PurchaseOrderID INT NOT NULL,
	ProductID INT NOT NULL,
	UnitOfMeasureCodeID INT NOT NULL,
	Quantity INT NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	CONSTRAINT PK_PurchaseOrderProduct PRIMARY KEY (PurchaseOrderProductID)
)

DROP TABLE IF EXISTS PurchaseOrderProductReceipt
CREATE TABLE PurchaseOrderProductReceipt (
	PurchaseOrderProductReceiptID INT IDENTITY(1, 1) NOT NULL,
	PurchaseOrderProductID INT NOT NULL,
	QuantityReceived INT NOT NULL,
	SerialNumbers VARCHAR(200) NOT NULL,
	CONSTRAINT PK_PurchaseOrderProductReceipt PRIMARY KEY (PurchaseOrderProductReceiptID)
)


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



