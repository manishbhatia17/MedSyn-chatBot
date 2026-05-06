


/* ======= This is 202004210958_CreateCustomerTables.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

DROP TABLE IF EXISTS Customer
CREATE TABLE Customer (
	CustomerID INT IDENTITY(1, 1) NOT NULL,
	CustomerName VARCHAR(100) NOT NULL,
	CustomerCustomID VARCHAR(100) NOT NULL,
	CustomerStatusCodeID INT NOT NULL,
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
	PracticeTypeCodeID INT NULL,
	PracticeTypeOther VARCHAR(100) NULL,
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
	SalesTaxCodeID INT NULL,
	OtherPaymentType VARCHAR(100),
	IsACHEDI BIT NOT NULL CONSTRAINT D_Customer_IsACHEDI DEFAULT(0),
	IsDomestic BIT NOT NULL CONSTRAINT D_Customer_IsDomestic DEFAULT(0),
	IsDomesticAfaxys BIT NOT NULL CONSTRAINT D_Customer_IsDomesticAfaxys DEFAULT(0),
	IsDomesticDistributor BIT NOT NULL CONSTRAINT D_Customer_IsDomesticDistributor DEFAULT(0),
	IsInternational BIT NOT NULL CONSTRAINT D_Customer_IsInternational DEFAULT(0),
	CreditCardName VARCHAR(100) NULL,
	CreditCardNumber VARCHAR(MAX) NULL,
	CVV VARCHAR(MAX) NULL,
	ExpirationDate VARCHAR(MAX) NULL,

	CONSTRAINT PK_Customer PRIMARY KEY (CustomerID)
)



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



