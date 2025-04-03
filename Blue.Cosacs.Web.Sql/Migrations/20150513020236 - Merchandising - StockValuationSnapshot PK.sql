-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Merchandising.StockValuationSnapshot ADD  CONSTRAINT [PK_Merchandising_StockValuationSnapshot] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,  ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


ALTER TABLE Merchandising.StockValuationSnapshot
ADD CONSTRAINT UC_StockValuationSnapshot UNIQUE (ProductId,LocationId, SnapshotDateId)

