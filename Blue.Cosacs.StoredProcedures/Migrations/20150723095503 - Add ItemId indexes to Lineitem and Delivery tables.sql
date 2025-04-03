-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'ix_DeliveryItemId' AND object_id = OBJECT_ID('Delivery'))
BEGIN
    CREATE NONCLUSTERED INDEX [ix_DeliveryItemId] ON [dbo].[Delivery]
    (
	    [ItemId] ASC
    )
     WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'ix_LineItemItemId' AND object_id = OBJECT_ID('Lineitem'))
BEGIN
    CREATE NONCLUSTERED INDEX [ix_LineItemItemId] ON [dbo].[lineitem]
    (
	    [ItemId] ASC
    )
     WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO