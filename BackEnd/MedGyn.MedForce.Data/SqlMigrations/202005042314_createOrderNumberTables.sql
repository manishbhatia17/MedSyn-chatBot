


/* ======= This is 202005042314_createOrderNumberTables.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


DROP TABLE IF EXISTS DailyPurchaseOrderCount
CREATE TABLE DailyPurchaseOrderCount (
	VendorID INT NOT NULL,
	LastCreated DATETIME2 NOT NULL,
	DailyCount INT NOT NULL
)

DROP TABLE IF EXISTS DailyCustomerOrderCount
CREATE TABLE DailyCustomerOrderCount (
	CustomerID INT NOT NULL,
	LastCreated DATETIME2 NOT NULL,
	DailyCount INT NOT NULL
)

ALTER TABLE PurchaseOrder ALTER COLUMN PurchaseOrderCustomID VARCHAR(100) NULL
ALTER TABLE CustomerOrder ALTER COLUMN CustomerOrderCustomID VARCHAR(100) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



