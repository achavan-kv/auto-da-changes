-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF EXISTS(select * from sys.indexes where name = 'IX_RetailPrice')
DROP INDEX [IX_RetailPrice] ON [Merchandising].[RetailPrice]



