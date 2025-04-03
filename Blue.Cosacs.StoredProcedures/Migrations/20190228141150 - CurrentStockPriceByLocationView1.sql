SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'CurrentStockPriceByLocationView1'
           AND xtype = 'U')
BEGIN 
CREATE  table [Merchandising].[CurrentStockPriceByLocationView1]
(
	Id int null,
	ProductId int not null,
	LocationId int not null,
	SalesId varchar(100) not null,
	RegularPrice decimal(19,4) null,
	CashPrice decimal(19,4) null,
	DutyFreePrice decimal(19,4) null,
	TaxRate decimal(15,4) null
)


END
GO