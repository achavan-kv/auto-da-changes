-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



ALTER TABLE Merchandising.CintOrderStats
ADD SecondaryReference varchar(20), ReferenceType varchar(20)

EXEC sp_rename 'Merchandising.CintOrderStats.Reference', 'PrimaryReference', 'COLUMN';
GO


ALTER TABLE Financial.CintOrderReceiptMessage
ADD SecondaryReference varchar(20), ReferenceType varchar(20)

EXEC sp_rename 'Financial.CintOrderReceiptMessage.Reference', 'PrimaryReference', 'COLUMN';
GO

ALTER TABLE Merchandising.CintError
ADD SecondaryReference varchar(20), ReferenceType varchar(20)

EXEC sp_rename 'Merchandising.CintError.ReferenceNumber', 'PrimaryReference', 'COLUMN';
GO


ALTER TABLE Merchandising.CintOrder
ADD SecondaryReference varchar(20), ReferenceType varchar(20)

EXEC sp_rename 'Merchandising.CintOrder.Reference', 'PrimaryReference', 'COLUMN';
GO