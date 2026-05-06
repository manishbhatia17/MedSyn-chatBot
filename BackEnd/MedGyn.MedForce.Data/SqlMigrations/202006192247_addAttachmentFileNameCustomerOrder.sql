


/* ======= This is 202006192247_addAttachmentFileNameCustomerOrder.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('CustomerOrder', 'AttachmentFileName') IS NULL
	ALTER TABLE CustomerOrder
		ADD AttachmentFileName VARCHAR(200) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



