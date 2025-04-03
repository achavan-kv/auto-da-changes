-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

CREATE TABLE [Merchandising].[CintOrderStats](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [varchar](50) NOT NULL,
	[Sku] varchar(20) NOT NULL,
	[ParentSku] varchar(20) NOT NULL,
	[StockLocation] varchar(50) NOT NULL,
	[QtyOrdered] [int] NOT NULL,
	[QtyDelivered] [int] NOT NULL,
	[QtyReturned] [int] NOT NULL,
	[QtyRepossessed] [int] NOT NULL,
 CONSTRAINT [pk_CintOrderStats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




