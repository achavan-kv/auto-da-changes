-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_StockPrice_StockInfo]') AND parent_object_id = OBJECT_ID(N'[dbo].[StockPrice]'))
ALTER TABLE [dbo].[StockPrice] DROP CONSTRAINT [fk_StockPrice_StockInfo]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_StockQuantity_StockInfo]') AND parent_object_id = OBJECT_ID(N'[dbo].[StockQuantity]'))
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [fk_StockQuantity_StockInfo]
GO

UPDATE StockQuantity
	set ID=i.ID 
From Stockinfo i INNER JOIN StockQuantity q on i.itemno=q.itemno
where q.id is null


IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'Trig_StockPrice_InsertUpdate')
BEGIN
	disable trigger Trig_StockPrice_InsertUpdate on StockPrice
END

go

UPDATE StockPrice
	set ID=i.ID 
From Stockinfo i INNER JOIN StockPrice p on i.itemno=p.itemno
where p.id is null
go

IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'Trig_StockPrice_InsertUpdate')
BEGIN
	enable trigger Trig_StockPrice_InsertUpdate on StockPrice
END

go 

if exists (select * from syscolumns where  name = 'ID'
               AND OBJECT_NAME(id) = 'StockPrice'
               and isnullable =1)
BEGIN
	alter TABLE StockPrice alter column ID INT not null
END  

if exists(select * from StockQuantity where id is null)
BEGIN
	UPDATE StockQuantity set id=1 where id is null
END

if exists (select * from syscolumns where  name = 'ID'
               AND OBJECT_NAME(id) = 'StockQuantity'
               and isnullable =1)
BEGIN
	alter TABLE StockQuantity alter column ID INT not null
END  
go 

ALTER TABLE [dbo].[StockQuantity]  WITH NOCHECK ADD  CONSTRAINT [fk_StockQuantity_StockInfo] FOREIGN KEY([ID])
REFERENCES [dbo].[StockInfo] ([ID])

ALTER TABLE [dbo].[StockPrice]  WITH NOCHECK ADD  CONSTRAINT [fk_StockPrice_StockInfo] FOREIGN KEY([ID])
REFERENCES [dbo].[StockInfo] ([ID]) 

           