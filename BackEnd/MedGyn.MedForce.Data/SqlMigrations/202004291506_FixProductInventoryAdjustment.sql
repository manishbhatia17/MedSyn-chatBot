


/* ======= This is 202004291506_FixProductInventoryAdjustment.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


DROP TABLE IF EXISTS ProductInventoryAdjustment
CREATE TABLE ProductInventoryAdjustment (
	ProductInventoryAdjustmentID INT IDENTITY(1, 1) NOT NULL,
	ProductID INT NOT NULL,
	Quantity INT NOT NULL,
	ReasonCodeID INT NOT NULL,
	ReasonCodeOther VARCHAR(MAX) NULL,
	AdjustmentDate DATETIME2 NOT NULL,
	AdjustedBy INT NOT NULL,
)




/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



