


/* ======= This is 202005261827_addPurchaseOrderDoNotReceiveReason.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

IF COL_LENGTH('PurchaseOrder', 'DoNotReceiveReason') IS NULL
	ALTER TABLE dbo.PurchaseOrder
		ADD DoNotReceiveReason VARCHAR(200) NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



