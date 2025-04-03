-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from NonStocks.NonStockType where Code = 'generic')
BEGIN
    insert into  NonStocks.NonStockType
    select 'generic'
END