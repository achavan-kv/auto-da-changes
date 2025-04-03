CREATE NONCLUSTERED INDEX IDX_Merchandising_CintOrder_Type
ON [Merchandising].[CintOrder] ([Type])
INCLUDE ([Id],[PrimaryReference],[Sku],[StockLocation],[ParentSku],[TransactionDate])
GO


CREATE NONCLUSTERED INDEX IDX_Merchandising_CintOrder_CintOrderSave
ON [Merchandising].[CintOrder] ([PrimaryReference],[Sku],[StockLocation],[ParentSku])
GO

