-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockInfo_InsertUpdate]'))
Disable TRIGGER [dbo].[Trig_StockInfo_InsertUpdate] on dbo.StockInfo
GO

UPDATE Stockinfo 
	set Class= case when c.category='PCF' then left(itemno,2) else left(itemno,3) end
from StockInfo s INNER JOIN Code c on s.category = CAST(c.code as INT) and c.category in('PCW', 'PCE', 'PCF','PCO')
where itemtype='S'

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockInfo_InsertUpdate]'))
Enable TRIGGER [dbo].[Trig_StockInfo_InsertUpdate] on dbo.StockInfo
GO
