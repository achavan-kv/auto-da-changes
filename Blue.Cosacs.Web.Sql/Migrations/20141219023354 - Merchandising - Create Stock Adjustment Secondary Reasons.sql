-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Merchandising].[StockAdjustmentSecondaryReason]') AND TYPE IN (N'U'))
DROP TABLE [Merchandising].[StockAdjustmentSecondaryReason]

create table Merchandising.[StockAdjustmentSecondaryReason] (
	Id int NOT NULL IDENTITY(1,1),
	PrimaryReasonId int NOT NULL,
	SecondaryReason varchar(100) NOT NULL,
	TransactionCode varchar(10) NOT NULL,
	DebitAccount varchar(30) NOT NULL,
	CreditAccount varchar(30) NOT NULL,
	SplitDebitByDepartment bit NOT NULL,
	SplitCreditByDepartment bit NOT NULL,
	DefaultForCountAjdustment bit NOT NULL,
	DateDeleted datetime NULL,	
	CONSTRAINT [PK_Merchandising_StockAdjustmentSecondaryReason] PRIMARY KEY CLUSTERED (Id ASC)
)

alter table Merchandising.[StockAdjustmentSecondaryReason] 
with check add constraint FK_Merchandising_StockAdjustmentSecondaryReason_PrimaryReason 
foreign key (PrimaryReasonId) 
references Merchandising.StockAdjustmentPrimaryReason(Id)
