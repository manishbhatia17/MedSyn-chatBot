


/* ======= This is 202004231649_AddCustomerShippingInfoTable.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

DROP TABLE IF EXISTS CustomerShippingInfo
CREATE TABLE CustomerShippingInfo (

	CustomerShippingInfoID INT IDENTITY(1, 1) NOT NULL,
	CustomerID INT NOT NULL,
	Name VARCHAR(100) NOT NULL,
	Address VARCHAR(100) NOT NULL,
	City VARCHAR(100) NOT NULL,
	StateCodeID INT NULL,
	ZipCode INT NULL,
	CountryCodeID INT NOT NULL,
	RepUserID INT NOT NULL,
	FedexAccountID INT NULL,
	UPSAccountID INT NULL,
	USMailAccountID INT NULL,
	DHLAccountID INT NULL,

	CONSTRAINT PK_CustomerShippingInfo PRIMARY KEY (CustomerShippingInfoID)
)




/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



