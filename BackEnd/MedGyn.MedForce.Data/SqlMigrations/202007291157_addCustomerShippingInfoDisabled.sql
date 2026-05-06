


/* ======= This is 202007291157_addCustomerShippingInfoDisabled.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

IF COL_LENGTH('CustomerShippingInfo', 'IsDisabled') IS NULL
	ALTER TABLE CustomerShippingInfo
		ADD IsDisabled BIT NULL
		CONSTRAINT D_CustomerShippingInfo_IsDisabled DEFAULT(0)

/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/
