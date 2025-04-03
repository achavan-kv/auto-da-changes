-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Merchandising.StockTransfer (
	Id int IDENTITY(1,1) NOT NULL,
	SendingLocationId int NOT NULL,
	RecevingLocationId int NOT NULL,
	ViaLocationId int NULL,
	Reference varchar(200) NOT NULL,
	Comments varchar(200) NULL,
	OriginalPrint datetime NULL,
	CreatedDate datetime NOT NULL,
	CreatedBy varchar(100) NOT NULL,
	CONSTRAINT [PK_StockTransfer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Merchandising].[StockTransfer]  WITH NOCHECK ADD  CONSTRAINT [FK_StockTransfer_SendingLocationId] FOREIGN KEY([SendingLocationId])
REFERENCES  [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[StockTransfer] CHECK CONSTRAINT [FK_StockTransfer_SendingLocationId]
GO

ALTER TABLE [Merchandising].[StockTransfer]  WITH NOCHECK ADD  CONSTRAINT [FK_StockTransfer_RecevingLocationId] FOREIGN KEY([RecevingLocationId])
REFERENCES [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[StockTransfer] CHECK CONSTRAINT [FK_StockTransfer_RecevingLocationId]
GO

ALTER TABLE [Merchandising].[StockTransfer]  WITH NOCHECK ADD  CONSTRAINT [FK_StockTransfer_ViaLocationId] FOREIGN KEY([ViaLocationId])
REFERENCES [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[StockTransfer] CHECK CONSTRAINT [FK_StockTransfer_ViaLocationId]
GO