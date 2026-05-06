-- Template for Product generation.
-- Version 0.1, generated at 202004020709  
-- Drop table, optional
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Product]') AND type in (N'U')) DROP TABLE Product
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Product]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Product](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
    [ProductName] VARCHAR(100) NOT NULL,  --Product Name
    [ProductIdentityText] VARCHAR(100) NOT NULL,  --Product Identity
    [UnitOfMeasureCode] INT ,  --Unit Of Measure Code
    [NetQuantity] INT ,  --Net Quantiy, Net=OnHand+PO-Commited
    [ReorderPoint] INT ,  --Reorder Point
    [ReorderQuantity] INT , 
    [PrimaryVendorID] INT NOT NULL,  --Primary Vendor ID (Lookup)
    [Cost] MONEY , 
    [InternationalOnly] BIT , 
    [SpecialOrderOnly] BIT , 
    [Color] VARCHAR(100) , 
    [ShipWeight] DECIMAL(10, 2) , 
    [Length] DECIMAL(10, 2) , 
    [Width] DECIMAL(10, 2) , 
    [Depth] DECIMAL(10, 2) , 
    [UMCode] INT ,  --Units of Measure Code
    [Description] VARCHAR(MAX) NOT NULL, 
    [Manufacturer] VARCHAR(100) , 
    [PriceDomesticList] MONEY , 
    [PriceDomesticDistribution] MONEY , 
    [PriceInternationalDistribution] MONEY , 
    [Notes] VARCHAR(MAX) , 
    [IsDeleted] BIT , 
    [UpdatedAt] DATETIME NOT NULL, 
    [UpdatedBy] VARCHAR(100) , 
 CONSTRAINT [PK_Product] PRIMARY KEY NONCLUSTERED 
(
	[ProductID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
-- Defaults

ALTER TABLE dbo.Product ADD DEFAULT (0) FOR [Cost]
ALTER TABLE dbo.Product ADD DEFAULT (0) FOR [InternationalOnly]
ALTER TABLE dbo.Product ADD DEFAULT (0) FOR [SpecialOrderOnly]
ALTER TABLE dbo.Product ADD DEFAULT (getdate()) FOR [UpdatedAt]
--Temporary 
ALTER TABLE dbo.Product ADD DEFAULT (1) FOR [PrimaryVendorID]
 
-- Add documentation if present - commented out since sp_addextendedproperty does not work on azure 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Product Name', 'schema', 'dbo', 'table',  'Product', 'column', 'ProductName'; 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Product Identity', 'schema', 'dbo', 'table',  'Product', 'column', 'ProductIdentityText'; 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Unit Of Measure Code', 'schema', 'dbo', 'table',  'Product', 'column', 'UnitOfMeasureCode'; 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Net Quantiy, Net=OnHand+PO-Commited', 'schema', 'dbo', 'table',  'Product', 'column', 'NetQuantity'; 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Reorder Point', 'schema', 'dbo', 'table',  'Product', 'column', 'ReorderPoint'; 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Primary Vendor ID (Lookup)', 'schema', 'dbo', 'table',  'Product', 'column', 'PrimaryVendorID'; 
--EXECUTE sys.sp_addextendedproperty 'MS_Description', 'Units of Measure Code', 'schema', 'dbo', 'table',  'Product', 'column', 'UMCode'; 
