


/* ======= This is 202004151827_FixProductTable.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


ALTER TABLE Product
	DROP COLUMN IF EXISTS NetQuantity;

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'IsDiscontinued' )
	ALTER TABLE Product ADD IsDiscontinued BIT NOT NULL
		CONSTRAINT D_Product_IsDiscontinued DEFAULT(0)



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



