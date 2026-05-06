


/* ======= This is 202007091204_makeZipNullable.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('SearchField', 'Customer') IS NULL
	ALTER TABLE Customer
		ADD SearchField VARCHAR(880) NULL

IF COL_LENGTH('SearchField', 'CustomerShippingInfo') IS NULL
	ALTER TABLE CustomerShippingInfo
		ADD SearchField VARCHAR(880) NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



