


/* ======= This is 202005311801_addFieldsForInvoice.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('CustomerOrderShipment', 'DeliveryDate') IS NULL
	ALTER TABLE CustomerOrderShipment
		ADD DeliveryDate DATETIME2 NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



