-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'ID' and object_name(id) = 'Fintrans')
BEGIN
	ALTER TABLE Fintrans ADD ID INT identity
END

IF NOT EXISTS(select * from syscolumns where name = 'FintransId' and object_name(id) = 'CashierTotalsBreakdown')
BEGIN
	ALTER TABLE CashierTotalsBreakdown ADD FintransId INT 
END

