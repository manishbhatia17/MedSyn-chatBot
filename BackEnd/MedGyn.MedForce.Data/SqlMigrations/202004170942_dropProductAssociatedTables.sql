


/* ======= This is 202004170942_dropProductAssociatedTables.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


DROP TABLE IF EXISTS ProductAdditionalVendor
DROP TABLE IF EXISTS ProductImage
DROP TABLE IF EXISTS ProductVendor

DROP TABLE IF EXISTS CodeType
CREATE TABLE CodeType (
	[CodeTypeID] INT NOT NULL,
	[CodeTypeName] VARCHAR(100) NOT NULL,
	[IsDeleted] BIT NOT NULL
)

ALTER TABLE Product DROP COLUMN IF EXISTS UnitOfMeasureCode
IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'UnitOfMeasureCodeID' )
	ALTER TABLE Product ADD UnitOfMeasureCodeID INT NULL


/* Additional Vendors */
IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'AdditionalVendor1ID' )
	ALTER TABLE Product ADD AdditionalVendor1ID INT NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'AdditionalVendor2ID' )
	ALTER TABLE Product ADD AdditionalVendor2ID INT NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'AdditionalVendor3ID' )
	ALTER TABLE Product ADD AdditionalVendor3ID INT NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'AdditionalVendor4ID' )
	ALTER TABLE Product ADD AdditionalVendor4ID INT NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'AdditionalVendor5ID' )
	ALTER TABLE Product ADD AdditionalVendor5ID INT NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'AdditionalVendor6ID' )
	ALTER TABLE Product ADD AdditionalVendor6ID INT NULL


/* Images */
IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'PrimaryImageURI' )
	ALTER TABLE Product ADD PrimaryImageURI VARCHAR(MAX) NULL
		CONSTRAINT D_REMOVE_ME_LATER DEFAULT('')

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage1URI' )
	ALTER TABLE Product ADD ExtraImage1URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage2URI' )
	ALTER TABLE Product ADD ExtraImage2URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage3URI' )
	ALTER TABLE Product ADD ExtraImage3URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage4URI' )
	ALTER TABLE Product ADD ExtraImage4URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage5URI' )
	ALTER TABLE Product ADD ExtraImage5URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage6URI' )
	ALTER TABLE Product ADD ExtraImage6URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage7URI' )
	ALTER TABLE Product ADD ExtraImage7URI VARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Product'
						AND COLUMN_NAME = 'ExtraImage8URI' )
	ALTER TABLE Product ADD ExtraImage8URI VARCHAR(MAX) NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



