


/* ======= This is 202004291708_EvenMoreProductTableChanges.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ProductIdentityText' )
	exec sp_rename '[Product].ProductIdentityText', 'ProductCustomID', 'COLUMN'

IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'UpdatedAt' )
	exec sp_rename '[Product].UpdatedAt', 'UpdatedOn', 'COLUMN'


UPDATE Product SET UpdatedBy = (SELECT TOP 1 UserID FROM [User])
ALTER TABLE Product
ALTER COLUMN UpdatedBy INT NOT NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



