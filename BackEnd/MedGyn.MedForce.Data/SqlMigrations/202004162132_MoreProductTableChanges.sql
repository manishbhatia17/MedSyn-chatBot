


/* ======= This is 202004162132_MoreProductTableChanges.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


ALTER TABLE Product	DROP COLUMN IF EXISTS UMCode;

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ShipWeightUnitsCodeID' )
	ALTER TABLE Product ADD ShipWeightUnitsCodeID INT NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ShipDimensionUnitsCodeID' )
	ALTER TABLE Product ADD ShipDimensionUnitsCodeID INT NULL


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



