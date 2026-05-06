


/* ======= This is 202006041355_addCOIntermediaryColumns.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('CustomerOrder', 'IntermediaryShippingName') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD IntermediaryShippingName VARCHAR(MAX) NULL

IF COL_LENGTH('CustomerOrder', 'IntermediaryShippingAddress') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD IntermediaryShippingAddress VARCHAR(MAX) NULL

IF COL_LENGTH('CustomerOrder', 'IntermediaryShippingContactNumber') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD IntermediaryShippingContactNumber VARCHAR(MAX) NULL

IF COL_LENGTH('CustomerOrder', 'IntermediaryShippingContactName') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD IntermediaryShippingContactName VARCHAR(MAX) NULL

IF COL_LENGTH('CustomerOrder', 'IntermediaryShippingContactEmail') IS NULL
	ALTER TABLE dbo.CustomerOrder
		ADD IntermediaryShippingContactEmail VARCHAR(MAX) NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



