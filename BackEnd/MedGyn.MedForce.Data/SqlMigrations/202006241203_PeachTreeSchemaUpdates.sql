/* ======= This is 202006241203_PeachTreeSchemaUpdates.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

IF NOT EXISTS (SELECT 1 FROM   sys.columns  WHERE  object_id = OBJECT_ID(N'[dbo].[CustomerOrderShipment]') AND name = 'AccountingExportBatchID')
ALTER TABLE dbo.CustomerOrderShipment ADD AccountingExportBatchID INT NULL;

IF NOT EXISTS (SELECT 1 FROM   sys.columns  WHERE  object_id = OBJECT_ID(N'[dbo].[PurchaseOrderProductReceipt]') AND name = 'AccountingExportBatchID')
ALTER TABLE dbo.PurchaseOrderProductReceipt ADD AccountingExportBatchID INT NULL;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[AccountingExportBatch]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AccountingExportBatch](
       [AccountingExportBatchID] [int] IDENTITY(1,1) NOT NULL,
       [ExportTS] [datetime] NOT NULL,
CONSTRAINT [PK_AccoutingExportBatch] PRIMARY KEY CLUSTERED 
(
       [AccountingExportBatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/