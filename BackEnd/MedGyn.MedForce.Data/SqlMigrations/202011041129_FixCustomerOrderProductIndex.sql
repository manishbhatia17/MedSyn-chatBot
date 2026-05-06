


DROP INDEX IF EXISTS [IX_CustomerOrderProduct_1] ON [dbo].[CustomerOrderProduct]
DROP INDEX IF EXISTS [IX_CustomerOrderProduct_CustomerOrderIDCustomerOrderProductIDProductID] ON [dbo].[CustomerOrderProduct]
CREATE UNIQUE CLUSTERED INDEX [IX_CustomerOrderProduct_CustomerOrderIDCustomerOrderProductIDProductID] ON [dbo].[CustomerOrderProduct]
(
	[CustomerOrderID] ASC,
	[CustomerOrderProductID] ASC,
	[ProductID] ASC
)
