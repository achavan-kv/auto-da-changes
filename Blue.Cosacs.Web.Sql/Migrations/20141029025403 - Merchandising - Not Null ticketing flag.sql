-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE [Merchandising].[Product] SET [PriceTicket] =0 WHERE [PriceTicket] IS NULL

ALTER TABLE [Merchandising].[Product] ALTER COLUMN [PriceTicket] BIT NOT NULL