


/* ======= This is 202006251103_makeZipNullable.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


ALTER TABLE [Customer]
	ALTER COLUMN ZipCode VARCHAR(50) NULL

ALTER TABLE [Vendor]
	ALTER COLUMN ZipCode VARCHAR(50) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



