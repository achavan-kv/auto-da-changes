-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Alter column type
ALTER TABLE Merchandising.StockValuationSnapshot 
ALTER column StockOnHandValue Decimal(19,4) not null 

ALTER TABLE Merchandising.StockValuationSnapshot 
ALTER column StockOnHandSalesValue Decimal(19,4) not null

