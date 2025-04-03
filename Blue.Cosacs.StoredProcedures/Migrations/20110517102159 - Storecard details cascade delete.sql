DECLARE @name NVARCHAR(100)

SELECT @name = name FROM sysobjects 
WHERE xtype = 'F'
AND name LIKE '%FK__StoreCard__Retai%'

SET @name = 'ALTER TABLE StoreCardRateDetails DROP CONSTRAINT ' + @name

IF EXISTS ( SELECT 1 FROM sysobjects 
			WHERE xtype = 'F'
			AND name LIKE '%FK__StoreCard__Retai%')
BEGIN
EXEC sp_executesql @name
END
GO

IF EXISTS (SELECT 1 FROM sysobjects 
		   WHERE xtype = 'F'
		   AND name = 'FK_StoreCard_StoreCardRate')
BEGIN
	ALTER TABLE StoreCardPaymentDetails
	DROP CONSTRAINT FK_StoreCard_StoreCardRate
END
GO

IF EXISTS (SELECT 1 FROM sysobjects 
		   WHERE xtype = 'F'
		   AND name = 'FK_StoreCardRateAudit_StoreCardRate')
BEGIN
	ALTER TABLE StoreCardRateAudit
	DROP CONSTRAINT FK_StoreCardRateAudit_StoreCardRate
END
GO

IF EXISTS (SELECT 1 FROM sysobjects 
		   WHERE xtype = 'F'
		   AND name = 'fk_StoreCardRate_ID')
BEGIN
	ALTER TABLE StoreCardRateDetails
	DROP CONSTRAINT fk_StoreCardRate_ID
END
GO

DROP TABLE StoreCardRateDetails
GO

TRUNCATE TABLE storecardrate 
GO

CREATE TABLE StoreCardRateDetails
(
	[Id] [int] NOT NULL,
	[ParentID] [int] NULL,
	[AppScoreFrom] [int] NULL,
	[AppScoreTo] [int] NULL,
	[PurchaseInterestRate] [dbo].[interest_rate] NULL,
	[BehaveScoreFrom] [smallint] NULL,
	[BehaveScoreTo] [smallint] NULL,
	[CreditLimitPercent] [decimal](5, 3) NULL,
	CONSTRAINT fk_StoreCardRate_ID
                FOREIGN KEY (ParentID)
                REFERENCES StoreCardRate(ID)
                ON DELETE CASCADE
)
GO
ALTER TABLE [dbo].[StoreCardRateDetails] ADD PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
GO

