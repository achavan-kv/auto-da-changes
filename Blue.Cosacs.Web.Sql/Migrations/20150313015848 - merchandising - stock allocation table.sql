CREATE TABLE Merchandising.StockAllocation (
	Id int IDENTITY(1,1) NOT NULL,
	WarehouseLocationId int NOT NULL,
	ReferenceNumber varchar(200) NULL,
	Comments varchar(200) NULL,
	CreatedDate datetime NOT NULL,
	CreatedById int NOT NULL,
	CONSTRAINT [PK_StockAllocation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Merchandising].StockAllocation  WITH NOCHECK ADD  CONSTRAINT [FK_StockAllocation_WarehouseLocationId] FOREIGN KEY([WarehouseLocationId])
REFERENCES  [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].StockAllocation CHECK CONSTRAINT [FK_StockAllocation_WarehouseLocationId]
GO
