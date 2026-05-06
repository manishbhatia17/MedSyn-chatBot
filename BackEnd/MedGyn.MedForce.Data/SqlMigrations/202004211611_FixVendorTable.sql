


/* ======= This is 202004211611_FixVendorTable.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

DROP TABLE IF EXISTS Vendor
CREATE TABLE Vendor (
	VendorID INT IDENTITY(1,1) NOT NULL,
	VendorName VARCHAR(100) NOT NULL,
	VendorCustomID VARCHAR(100) NULL,
	Address1 VARCHAR(100) NOT NULL,
	Address2 VARCHAR(100) NULL,
	City VARCHAR(100) NOT NULL,
	StateCodeID INT NULL,
	ZipCode INT NOT NULL,
	CountryCodeID INT NOT NULL,
	Website VARCHAR(100) NULL,
	PrimaryContact VARCHAR(100) NULL,
	PrimaryEmail VARCHAR(100) NULL,
	PrimaryPhone VARCHAR(15) NOT NULL,
	PrimaryFax VARCHAR(15) NULL,
	TaxID VARCHAR(100) NULL,
	AdditionalContact1Name VARCHAR(100) NULL,
	AdditionalContact1Email VARCHAR(100) NULL,
	AdditionalContact1Phone VARCHAR(15) NULL,
	AdditionalContact2Name VARCHAR(100) NULL,
	AdditionalContact2Email VARCHAR(100) NULL,
	AdditionalContact2Phone VARCHAR(15) NULL,
	AdditionalContact3Name VARCHAR(100) NULL,
	AdditionalContact3Email VARCHAR(100) NULL,
	AdditionalContact3Phone VARCHAR(15) NULL,
	PaymentTermsType INT NOT NULL,
	PaymentTermsNetDueDays INT NULL,
	BankRoutingNumber VARCHAR(100) NULL,
	BankAccountNumber VARCHAR(MAX) NULL,
	BankAddress VARCHAR(100) NULL,
	BankCity VARCHAR(100) NULL,
	BankStateCodeID INT NULL,
	BankZipCode INT NULL,
	MinOrderAmount DECIMAL(10,2) NULL,
	CreditLimit VARCHAR(100) NULL,
	GLAccountNumberCodeID INT NULL,
	Notes VARCHAR(MAX) NULL,
	CertificationStatus VARCHAR(100) NULL,
	QualityInformation VARCHAR(MAX) NULL,
	Components VARCHAR(MAX) NULL,
	UpdatedBy INT NOT NULL,
	UpdatedOn DATETIME2 NOT NULL,
	CONSTRAINT PK_Vendor PRIMARY KEY (VendorID)
)


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



