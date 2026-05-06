


/* ======= This is 202005061207_addColumnsToCustomerOrder.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('CustomerOrder', 'CreatedBy') IS NULL
	ALTER TABLE CustomerOrder
	ADD CreatedBy INT NOT NULL

IF COL_LENGTH('CustomerOrder', 'CreatedOn') IS NULL
	ALTER TABLE CustomerOrder
	ADD CreatedOn DATETIME2 NOT NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



