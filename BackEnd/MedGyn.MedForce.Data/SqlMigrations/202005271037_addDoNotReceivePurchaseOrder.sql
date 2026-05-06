
/* ======= This is 202005271037_addDoNotReceivePurchaseOrder.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

---------------------- Domestic Columns---------------------
IF COL_LENGTH('PurchaseOrder', 'IsDoNotReceive') IS NULL
	ALTER TABLE dbo.PurchaseOrder
		ADD IsDoNotReceive BIT NOT NULL
		CONSTRAINT D_PurchaseOrder_IsDoNotReceive DEFAULT(0)



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



