-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Merchandising.NegativeStockSnapshot
(
Id int identity(1,1)  
,ProductId	int	    not null 
,LocationId	int	  not null      
,StockOnHandQuantity	int not null 
,AverageWeightedCost	decimal(19,4)   not null 
,StockOnHandValue	decimal(19,4) not null  
,StockOnHandSalesValue	decimal(19,4)  not null   
,SnapshotDateId int not null
)

ALTER TABLE Merchandising.NegativeStockSnapshot ADD  CONSTRAINT [PK_Merchandising_NegativeStockSnapshot] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,  ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [Merchandising].NegativeStockSnapshot
WITH CHECK ADD CONSTRAINT FK_NegativeStock_ProductId_Product
FOREIGN KEY (ProductId)
REFERENCES [Merchandising].Product(Id) 

ALTER TABLE [Merchandising].NegativeStockSnapshot
WITH CHECK ADD CONSTRAINT FK_NegativeStock_LocationId_Location
FOREIGN KEY (LocationId)
REFERENCES [Merchandising].Location(Id)

ALTER TABLE Merchandising.NegativeStockSnapshot
ADD CONSTRAINT UC_NegativeStockSnapshot UNIQUE (ProductId,LocationId, SnapshotDateId)
