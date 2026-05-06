IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'UpdatedBy'
          AND Object_ID = Object_ID(N'Vendor'))
BEGIN
    -- Column Does Not Exist
	ALTER TABLE [dbo].Vendor
    ADD UpdatedBy [varchar](100) NULL
END

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'UpdatedAt'
          AND Object_ID = Object_ID(N'Vendor'))
BEGIN
    -- Column Does Not Exist
	ALTER TABLE [dbo].Vendor
    ADD UpdatedAt [datetime] NULL
END