-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE NONCLUSTERED INDEX IDX_MerchandisingProduct_ProductType_Id_SKU
ON [Merchandising].[Product] ([ProductType])
INCLUDE ([Id], [SKU])
GO