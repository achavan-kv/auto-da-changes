CREATE TABLE Merchandising.StockRequisition (
	Id int IDENTITY(1,1) NOT NULL,
	WarehouseLocationId int NOT NULL,
	ReceivingLocationId int NOT NULL,
	Reference varchar(200) NOT NULL,
	Comments varchar(200) NULL,
	CreatedDate datetime NOT NULL,
	CreatedById int NOT NULL,
	CONSTRAINT [PK_StockRequisition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Merchandising].[StockRequisition]  WITH NOCHECK ADD  CONSTRAINT [FK_StockRequisition_WarehouseLocationId] FOREIGN KEY([WarehouseLocationId])
REFERENCES  [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[StockRequisition] CHECK CONSTRAINT [FK_StockRequisition_WarehouseLocationId]
GO

ALTER TABLE [Merchandising].[StockRequisition]  WITH NOCHECK ADD  CONSTRAINT [FK_StockRequisition_ReceivingLocationId] FOREIGN KEY([ReceivingLocationId])
REFERENCES [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[StockRequisition] CHECK CONSTRAINT [FK_StockRequisition_ReceivingLocationId]
GO