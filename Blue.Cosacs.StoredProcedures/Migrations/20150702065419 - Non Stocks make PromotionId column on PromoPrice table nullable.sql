-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'PromoPrice' AND  Column_Name = 'PromotionId')
BEGIN
	ALTER TABLE PromoPrice Alter column PromotionId int NULL
END