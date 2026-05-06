


/* ======= This is 202004291755_updatePhoneNumberColumns.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

ALTER TABLE Vendor ALTER COLUMN PrimaryPhone VARCHAR(50) NOT NULL
ALTER TABLE Vendor ALTER COLUMN PrimaryFax VARCHAR(50) NULL
ALTER TABLE Vendor ALTER COLUMN AdditionalContact1Phone VARCHAR(50) NULL
ALTER TABLE Vendor ALTER COLUMN AdditionalContact2Phone VARCHAR(50) NULL
ALTER TABLE Vendor ALTER COLUMN AdditionalContact3Phone VARCHAR(50) NULL

ALTER TABLE Customer ALTER COLUMN PrimaryPhone VARCHAR(50) NOT NULL
ALTER TABLE Customer ALTER COLUMN PrimaryFax VARCHAR(50) NULL
ALTER TABLE Customer ALTER COLUMN AdditionalContact1Phone VARCHAR(50) NULL
ALTER TABLE Customer ALTER COLUMN AdditionalContact2Phone VARCHAR(50) NULL
ALTER TABLE Customer ALTER COLUMN AdditionalContact3Phone VARCHAR(50) NULL




/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



