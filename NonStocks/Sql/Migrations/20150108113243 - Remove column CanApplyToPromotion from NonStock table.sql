-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock' AND Column_Name = 'CanApplyToPromotion'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock DROP COLUMN CanApplyToPromotion
END
GO