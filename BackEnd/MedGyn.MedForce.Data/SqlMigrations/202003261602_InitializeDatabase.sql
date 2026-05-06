/****** Object:  Table [_DBVersion]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[_DBVersion]') AND type in (N'U'))
BEGIN
CREATE TABLE [_DBVersion](
	[DBVersionId] [int] IDENTITY(1,1) NOT NULL,
	[Version] [varchar](16) NULL,
	[RunDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DBVersionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [Code]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Code]') AND type in (N'U'))
BEGIN
CREATE TABLE [Code](
	[CodeId] [int] IDENTITY(1,1) NOT NULL,
	[CodeName] [varchar](max) NULL,
	[CodeDescription] [varchar](max) NULL,
	[CodeTypeId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Code] PRIMARY KEY NONCLUSTERED 
(
	[CodeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

/****** Object:  Table [CodeType]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CodeType]') AND type in (N'U'))
BEGIN
CREATE TABLE [CodeType](
	[CodeTypeId] [int] IDENTITY(1,1) NOT NULL,
	[CodeTypeName] [varchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CodeType] PRIMARY KEY NONCLUSTERED 
(
	[CodeTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [CustomerOrderLine]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerOrderLine]') AND type in (N'U'))
BEGIN
CREATE TABLE [CustomerOrderLine](
	[CustomerOrderLineID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerOrderID] [int] NOT NULL,
	[CustomerOrderLineSeq] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [PK_CustomerOrderLine] PRIMARY KEY NONCLUSTERED 
(
	[CustomerOrderLineID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [CustomerOrderLineFill]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerOrderLineFill]') AND type in (N'U'))
BEGIN
CREATE TABLE [CustomerOrderLineFill](
	[CustomerOrderLineFillID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerOrderLineID] [int] NOT NULL,
	[FilledTS] [timestamp] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_CustomerOrderLineReceipt] PRIMARY KEY NONCLUSTERED 
(
	[CustomerOrderLineFillID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [Product]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Product]') AND type in (N'U'))
BEGIN
CREATE TABLE [Product](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [varchar](100) NOT NULL,
	[ProductIdentityText] [varchar](100) NOT NULL,
	[InternationalOnly] [bit] NULL,
	[SpecialOrderOnly] [bit] NULL,
	[UnitOfMeasureCode] [int] NULL,
	[Color] [varchar](100) NULL,
	[ShipWeight] [decimal](10, 2) NULL,
	[Length] [decimal](10, 2) NULL,
	[LengthUMCode] [int] NULL,
	[Width] [decimal](10, 2) NULL,
	[WidthUMCode] [int] NULL,
	[Depth] [decimal](10, 2) NULL,
	[DepthUMCode] [int] NULL,
	[PrimaryVendorID] [int] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[Manufacturer] [varchar](100) NULL,
	[PriceDomesticList] [money] NULL,
	[PriceDomesticDistribution] [money] NULL,
	[PriceInternationalDistribution] [money] NULL,
	[Cost] [money] NULL,
	[Notes] [varchar](max) NULL,
	[ReorderPoint] [int] NULL,
	[ReorderQuantity] [int] NULL,
	[IsDeleted] [bit] NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[UpdatedBy] [varchar](100) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY NONCLUSTERED 
(
	[ProductID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

/****** Object:  Table [ProductAdditionalVendor]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductAdditionalVendor]') AND type in (N'U'))
BEGIN
CREATE TABLE [ProductAdditionalVendor](
	[ProductAddtionalVendorID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[VendorID] [int] NOT NULL,
 CONSTRAINT [PK_ProductAdditionalVendor] PRIMARY KEY NONCLUSTERED 
(
	[ProductAddtionalVendorID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [ProductImage]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [ProductImage](
	[ProductImageID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[Sequence] [int] NOT NULL,
	[ImageURI] [varchar](1000) NOT NULL,
 CONSTRAINT [PK_ProductImage] PRIMARY KEY NONCLUSTERED 
(
	[ProductImageID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [ProductInventoryAdjustment]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductInventoryAdjustment]') AND type in (N'U'))
BEGIN
CREATE TABLE [ProductInventoryAdjustment](
	[ProductInventoryAdjustmentID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Reason] [varchar](max) NULL,
 CONSTRAINT [PK_ProductInventoryAdjustment] PRIMARY KEY NONCLUSTERED 
(
	[ProductInventoryAdjustmentID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

/****** Object:  Table [ProductVendor]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductVendor]') AND type in (N'U'))
BEGIN
CREATE TABLE [ProductVendor](
	[ProductVendorID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[VendorID] [int] NOT NULL,
 CONSTRAINT [PK_ProductVendor] PRIMARY KEY NONCLUSTERED 
(
	[ProductVendorID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [PurchaseOrderLine]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PurchaseOrderLine]') AND type in (N'U'))
BEGIN
CREATE TABLE [PurchaseOrderLine](
	[ProductOrderLineID] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderID] [int] NOT NULL,
	[PurchaseOrderLineSeq] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [PK_PurchaseOrderLine] PRIMARY KEY NONCLUSTERED 
(
	[ProductOrderLineID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [PurchaseOrderLineReceipt]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PurchaseOrderLineReceipt]') AND type in (N'U'))
BEGIN
CREATE TABLE [PurchaseOrderLineReceipt](
	[PurchaseOrderLineReceiptID] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderLineID] [int] NOT NULL,
	[ReceivedTS] [timestamp] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOrderLineReceipt] PRIMARY KEY NONCLUSTERED 
(
	[PurchaseOrderLineReceiptID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [Role]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Role]') AND type in (N'U'))
BEGIN
CREATE TABLE [Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY NONCLUSTERED 
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [RoleSecurityKey]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[RoleSecurityKey]') AND type in (N'U'))
BEGIN
CREATE TABLE [RoleSecurityKey](
	[RoleSecurityKeyId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[SecurityKeyId] [int] NOT NULL,
 CONSTRAINT [PK_RoleSecurity_Key] PRIMARY KEY NONCLUSTERED 
(
	[RoleSecurityKeyId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [SecurityKey]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SecurityKey]') AND type in (N'U'))
BEGIN
CREATE TABLE [SecurityKey](
	[SecurityKeyId] [int] IDENTITY(1,1) NOT NULL,
	[SecurityKeyName] [varchar](100) NOT NULL,
	[DisplayOrder] [char](10) NOT NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_SecurityKey] PRIMARY KEY NONCLUSTERED 
(
	[SecurityKeyId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Table [Tester]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tester]') AND type in (N'U'))
BEGIN
CREATE TABLE [Tester](
	[Id] [int] NULL,
	[Name] [nchar](10) NULL,
	[Tester] [bit] NULL
) ON [PRIMARY]
END

/****** Object:  Table [User]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](500) NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[Password] [varbinary](128) NULL,
	[PasswordSalt] [varbinary](max) NULL,
	[ForcePasswordReset] [bit] NOT NULL,
	[ResetPasswordOn] [datetime] NULL,
	[RoleId] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[SalesRepId] [int] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

/****** Object:  Table [Vendor]    Script Date: 3/27/2020 11:24:48 AM ******/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Vendor]') AND type in (N'U'))
BEGIN
CREATE TABLE [Vendor](
	[VendorID] [int] IDENTITY(1,1) NOT NULL,
	[VendorName] [varchar](100) NOT NULL,
	[VendorIdentity] [varchar](100) NULL,
	[Address1] [varchar](100) NULL,
	[Address2] [varchar](100) NULL,
	[City] [varchar](100) NULL,
	[StateCodeID] [int] NULL,
	[Zip] [varchar](10) NULL,
	[Country] [varchar](100) NULL,
	[Website] [varchar](100) NULL,
	[PrimaryContact] [varchar](100) NULL,
	[PrimaryEmail] [varchar](100) NULL,
	[PrimaryPhone] [varchar](100) NULL,
	[PrimaryFax] [varchar](100) NULL,
	[TaxID] [varchar](10) NULL,
	[Contact1Name] [varchar](100) NULL,
	[Contact1Email] [varchar](100) NULL,
	[Contact1Phone] [varchar](100) NULL,
	[Contact2Name] [varchar](100) NULL,
	[Contact2Email] [varchar](100) NULL,
	[Contact2Phone] [varchar](100) NULL,
	[Contact3Name] [varchar](100) NULL,
	[Contact3Email] [varchar](100) NULL,
	[Contact3Phone] [varchar](100) NULL,
	[PaymentTerms] [varchar](20) NULL,
	[SwiftABARoutingNumber] [varchar](100) NULL,
	[BankRoutingNumber] [varchar](100) NULL,
	[BankAddress] [varchar](100) NULL,
	[BankCity] [varchar](100) NULL,
	[BankStateCodeID] [int] NULL,
	[BankZip] [varchar](10) NULL,
	[MinimumOrderAmount] [int] NULL,
	[CreditLimit] [int] NULL,
	[GLAccountNumber] [varchar](100) NULL,
	[NotesFinancial] [varchar](max) NULL,
	[CertificationStatus] [varchar](100) NULL,
	[QualityInformation] [varchar](max) NULL,
	[Components] [varchar](max) NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY NONCLUSTERED 
(
	[VendorID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF___DBVersio__RunDa__2180FB33]') AND type = 'D')
BEGIN
ALTER TABLE [_DBVersion] ADD  DEFAULT (getdate()) FOR [RunDate]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF__Product__Updated__22751F6C]') AND type = 'D')
BEGIN
ALTER TABLE [Product] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
END

