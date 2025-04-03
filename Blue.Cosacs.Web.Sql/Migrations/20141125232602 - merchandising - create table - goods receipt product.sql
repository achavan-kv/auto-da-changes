IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Merchandising].[GoodsReceiptProduct]') AND TYPE IN (N'U'))
DROP TABLE [Merchandising].[GoodsReceiptProduct]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Merchandising].[GoodsReceiptPurchaseOrder]') AND TYPE IN (N'U'))
DROP TABLE [Merchandising].[GoodsReceiptPurchaseOrder]

create table Merchandising.GoodsReceiptProduct (
	Id int NOT NULL IDENTITY(1,1),
	
	GoodsReceiptId int NOT NULL,
	PurchaseOrderProductId int NOT NULL,

	QuantityReceived int not null,
	QuantityBackOrdered int not null,
	ReasonForCancellation varchar(100) null,

	CONSTRAINT [PK_Merchandising_GoodsReceiptProduct] PRIMARY KEY CLUSTERED (Id ASC),
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_Merchandising_GoodsReceiptProduct] ON Merchandising.GoodsReceiptProduct (GoodsReceiptId, PurchaseOrderProductId)

alter table Merchandising.GoodsReceiptProduct with check add constraint FK_Merchandising_GoodsReceiptProduct_GoodsReceipt foreign key (GoodsReceiptId) references Merchandising.GoodsReceipt(Id)
alter table Merchandising.GoodsReceiptProduct with check add constraint FK_Merchandising_GoodsReceiptProduct_PurchaseOrderProduct foreign key (PurchaseOrderProductId) references Merchandising.PurchaseOrderProduct(Id)