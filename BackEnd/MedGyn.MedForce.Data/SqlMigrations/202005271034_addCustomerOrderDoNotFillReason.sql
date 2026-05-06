


/* ======= This is 202005271034_addCustomerOrderDoNotFillReason.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

IF COL_LENGTH('CustomerOrder', 'DoNotFillReason') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD DoNotFillReason VARCHAR(200) NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



