-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Credit.StoreCardImport
	DROP CONSTRAINT UQ_StoreCardImportTransaction

ALTER TABLE Credit.StoreCardImport
	ALTER COLUMN TransactionDate DateTime NOT NULL

ALTER TABLE Credit.StoreCardImport
	ADD CONSTRAINT UQ_StoreCardImportTransaction UNIQUE (CardNumber, TransactionDate)