-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF OBJECT_ID('Financial.WarrantyMessage') IS NOT NULL
	DROP TABLE Financial.WarrantyMessage
GO

CREATE TABLE Financial.WarrantyMessage
(
	id				Int IDENTITY (1, 1)	NOT NULL,
	ContractNumber	VarChar(20)			NOT NULL,
	AccountType		Char(3)				NOT NULL,
	Department		Char(3)				NOT NULL,
	SalePrice		Decimal(12, 4)		NOT NULL,
	BranchNo		SmallInt			NOT NULL,
	MessageId		Int					NOT NULL
)
GO

ALTER TABLE Financial.WarrantyMessage ADD CONSTRAINT PK_Financial_WarrantyMessage_id PRIMARY KEY CLUSTERED 
(
	id ASC
)WITH 
(
	PAD_INDEX = OFF, 
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
)
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_Financial_WarrantyMessage_ContractNumber ON Financial.WarrantyMessage
(
	ContractNumber DESC
)WITH 
(
	IGNORE_DUP_KEY = OFF,
	PAD_INDEX = ON, 
	FILLFACTOR = 70,
	STATISTICS_NORECOMPUTE = OFF, 
	SORT_IN_TEMPDB = OFF, 
	DROP_EXISTING = OFF, 
	ONLINE = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
)
GO


