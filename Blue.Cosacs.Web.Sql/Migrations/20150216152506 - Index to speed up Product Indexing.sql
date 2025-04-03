-- transaction: true

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_StockQuantity_deleted_stocklocn_ID' AND object_id = OBJECT_ID('[dbo].[StockQuantity]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_StockQuantity_deleted_stocklocn_ID]
    ON [dbo].[StockQuantity] ([deleted])
    INCLUDE ([stocklocn],[ID])
END
