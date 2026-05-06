


/* ======= This is 202006251002_addAddress2CustomerShipping.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('Address2', 'CustomerShippingInfo') IS NULL
	ALTER TABLE CustomerShippingInfo
		ADD Address2 VARCHAR(100) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



