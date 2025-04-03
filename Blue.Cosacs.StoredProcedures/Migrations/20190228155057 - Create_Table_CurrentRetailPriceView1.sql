SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'CurrentRetailPriceView1'
           AND xtype = 'U')
BEGIN 

CREATE TABLE [Warranty].[CurrentRetailPriceView1](
	[Id] [int] NULL,
	[LocationId] [int] NULL,
	[ProductId] [int] NOT NULL,
	[EffectiveDate] [date] NULL,
	[RegularPrice] [decimal](38, 4) NULL,
	[CashPrice] [decimal](38, 4) NULL,
	[DutyFreePrice] [decimal](38, 4) NULL,
	[Fascia] [varchar](100) NULL,
	[TaxRate] [decimal](38, 6) NULL,
	[Name] [varchar](100) NULL
) ON [PRIMARY]

END
GO


