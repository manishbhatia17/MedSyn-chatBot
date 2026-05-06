


/* ======= This is 202006051842_addInvoiceTotalToCustomerShipment.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('CustomerOrderShipment', 'InvoiceTotal') IS NULL
	ALTER TABLE CustomerOrderShipment
		ADD InvoiceTotal DECIMAL(10,2) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



