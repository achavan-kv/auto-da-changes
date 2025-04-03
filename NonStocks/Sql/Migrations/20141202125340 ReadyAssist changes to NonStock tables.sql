-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from NonStocks.NonStockType where Code = 'rassist')
BEGIN
    insert into  NonStocks.NonStockType
    select 'rassist'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock' AND Column_Name = 'Length'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock ADD Length INT NULL
END

