-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


DROP INDEX [IX_ComboProductPrice] ON [Merchandising].[ComboProductPrice]
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_ComboProductPrice] ON [Merchandising].[ComboProductPrice]
(
	[ComboProductId] ASC,
	[LocationId] ASC,
	[Fascia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

