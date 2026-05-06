IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SalesRepId'
          AND Object_ID = Object_ID(N'User'))
BEGIN
    -- Column Exists
	ALTER TABLE [dbo].[User]
    ADD SalesRepId [int] NULL
END