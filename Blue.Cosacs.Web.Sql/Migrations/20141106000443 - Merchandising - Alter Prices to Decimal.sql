-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


ALTER TABLE Merchandising.RetailPrice
ALTER COLUMN RegularPrice Decimal(15,4) not null

ALTER TABLE Merchandising.RetailPrice
ALTER COLUMN CashPrice Decimal(15,4) not null

ALTER TABLE Merchandising.RetailPrice
ALTER COLUMN DutyFreePrice Decimal(15,4) not null