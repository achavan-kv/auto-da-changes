SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'CurrentStockRetailPriceView1'
           AND xtype = 'U')
BEGIN 

CREATE TABLE [Merchandising].[CurrentStockRetailPriceView1](
	[Id] [int] NOT NULL,
	[LocationId] [int] NULL,
	[ProductId] [int] NOT NULL,
	[EffectiveDate] [date] NOT NULL,
	[RegularPrice] [decimal](15, 4) NOT NULL,
	[CashPrice] [decimal](15, 4) NOT NULL,
	[DutyFreePrice] [decimal](15, 4) NOT NULL,
	[Fascia] [varchar](100) NULL,
	[TaxRate] [decimal](15, 4) NOT NULL,
	[Name] [varchar](100) NULL
) ON [PRIMARY]


END
GO
