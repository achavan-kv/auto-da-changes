-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Merchandising.PromotionDetail
ALTER COLUMN Price Decimal(15,4) null

ALTER TABLE Merchandising.PromotionDetail
ALTER COLUMN ValueDiscount Decimal(15,4) null
