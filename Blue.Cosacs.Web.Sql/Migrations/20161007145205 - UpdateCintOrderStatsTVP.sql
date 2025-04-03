
IF TYPE_ID(N'Merchandising.UpdateCintOrderStatsTVP') IS NOT NULL 
DROP TYPE Merchandising.UpdateCintOrderStatsTVP
GO


CREATE TYPE Merchandising.UpdateCintOrderStatsTVP AS TABLE(
	[PrimaryReference] [varchar](50) NOT NULL,
	[Sku] [varchar](20) NOT NULL,
	[ParentSku] [varchar](20) NOT NULL,
	[StockLocation] [varchar](50) NOT NULL,
	QtyOrderedInc [int] NOT NULL ,
	QtyDeliveredInc [int] NOT NULL,
	QtyReturnedInc [int] NOT NULL,
	QtyRepossessedInc [int] NOT NULL ,
	QtyOrdered int NOT NULL,
	[SecondaryReference] [varchar](20) NULL,
	[ReferenceType] [varchar](20) NULL
)
GO
