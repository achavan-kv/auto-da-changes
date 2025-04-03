EXECUTE sp_rename N'dbo.StoreCard.Number', N'Tmp_CardNumber', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.StoreCard.Name', N'Tmp_CardName_1', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.StoreCard.Tmp_CardNumber', N'CardNumber', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.StoreCard.Tmp_CardName_1', N'CardName', 'COLUMN' 
GO

ALTER TABLE dbo.StoreCard
	DROP CONSTRAINT FK_StoreCard_custacct
GO
ALTER TABLE dbo.StoreCard
	DROP COLUMN CustId
GO
