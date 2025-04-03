-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Merchandising].[StockAdjustmentSecondaryReason]') AND TYPE IN (N'U'))
DROP TABLE [Merchandising].[StockAdjustmentSecondaryReason]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Merchandising].[StockAdjustmentPrimaryReason]') AND TYPE IN (N'U'))
DROP TABLE [Merchandising].[StockAdjustmentPrimaryReason]

create table Merchandising.[StockAdjustmentPrimaryReason] (
	Id int NOT NULL IDENTITY(1,1),	
	Name varchar(100) NOT NULL,
	DateDeleted datetime,	
	CONSTRAINT [PK_Merchandising_StockAdjustmentPrimaryReason] PRIMARY KEY CLUSTERED (Id ASC)
)
