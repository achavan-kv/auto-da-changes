-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE FROM Financial.TransferProductMessage

DELETE FROM  Financial.TransferMessage

ALTER TABLE Financial.TransferMessage
ADD [Type] varchar(100) NOT NULL
	,ReceivingLocationId INT NOT NULL
	,ReceivingSalesId varchar(100) NOT NULL
