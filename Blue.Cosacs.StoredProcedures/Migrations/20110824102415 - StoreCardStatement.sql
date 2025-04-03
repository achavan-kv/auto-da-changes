IF EXISTS (SELECT * FROM sysobjects
		   WHERE name = 'StoreCardStatements'
		   AND xtype = 'U')
BEGIN
	DROP TABLE StoreCardStatements
END
GO

IF EXISTS (SELECT * FROM sysobjects
		   WHERE name = 'StoreCardStatement'
		   AND xtype = 'U')
BEGIN
	DROP TABLE StoreCardStatement
END
GO

CREATE TABLE StoreCardStatement
(
	Id INT IDENTITY(1,1) NOT NULL,
	CustID VARCHAR(20) NOT NULL,
	[Acctno] [char](12) NOT NULL,
	[DateFrom] [smalldatetime] NOT NULL,
	[DateTo] [smalldatetime] NOT NULL,
	[DatePrinted] [smalldatetime] NULL,
	[DateLastReprinted] [smalldatetime] NULL,
	[ReprintedBy] [int] NULL
) 

GO

ALTER TABLE [dbo].[StoreCardStatement]  WITH CHECK ADD  CONSTRAINT [FK_StoreCardStatements_StoreCardPaymentDetails] FOREIGN KEY([Acctno])
REFERENCES [dbo].[StoreCardPaymentDetails] ([acctno])
GO

ALTER TABLE [dbo].[StoreCardStatement] CHECK CONSTRAINT [FK_StoreCardStatements_StoreCardPaymentDetails]
GO

ALTER TABLE dbo.StoreCardStatement ADD CONSTRAINT
	PK_StoreCardStatements PRIMARY KEY CLUSTERED 
	(
	Id
	) 
GO
