IF EXISTS (SELECT * FROM sysobjects 
	       WHERE xtype = 'U'
	       AND name = 'StoreCardRateAudit')
BEGIN
	DROP TABLE StoreCardRateAudit
END
GO

CREATE TABLE StoreCardRateAudit
(
	[Id] [int] NOT NULL,
	[$Action] [char](1) NOT NULL,
	[$CreatedOn] [datetime] NULL,
	[$CreatedBy] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[RateFixed]  BIT NULL, 
	[$AuditId] [int] IDENTITY(1,1) NOT NULL,
	[AppScorefrom] [smallint] NULL,
	[AppScoreTo] [smallint] NULL,
	[BehaveScoreFrom] [smallint] NULL,
	[BehaveScoreTo] [smallint] NULL,
	PurchaseInterestRate DECIMAL NOT NULL
)
 GO
 
ALTER TABLE StoreCardRateAudit  
ADD CONSTRAINT PK_StoreCardRateAudit PRIMARY KEY CLUSTERED 
(
	[$AuditId] ASC
)
GO

