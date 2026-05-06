


/* ======= This is 202006040908_VendorStatusCodes.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('Vendor', 'VendorStatusCodeID') IS NULL
	ALTER TABLE Vendor
	ADD VendorStatusCodeID INT NULL

IF NOT EXISTS(SELECT 1 FROM Code WHERE CodeTypeID = 24 AND CodeName = 'Active')
	INSERT INTO Code (CodeName, CodeDescription, CodeTypeID, IsRequired, IsDeleted) VALUES
		('Active', 'Active', 24, 1, 0)

-- This needs to run in EXEC for new column to exist
EXEC('
UPDATE Vendor
	SET VendorStatusCodeID = (
		SELECT TOP 1 CodeID FROM Code
		WHERE CodeTypeID = 24 AND CodeName = ''Active''
	)
	WHERE VendorStatusCodeID IS NULL
')

EXEC('ALTER TABLE Vendor ALTER COLUMN VendorStatusCodeID INT NOT NULL')


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



