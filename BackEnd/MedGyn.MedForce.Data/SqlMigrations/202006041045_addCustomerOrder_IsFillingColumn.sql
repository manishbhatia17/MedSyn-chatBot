


/* ======= This is 202006041045_addCustomerOrder_IsFillingColumn.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('CustomerOrder', 'IsFilling') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD IsFilling BIT NOT NULL
		CONSTRAINT D_CustomerOrder_IsFilling DEFAULT(0)



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



