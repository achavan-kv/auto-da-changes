-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Merchandising.GoodsReceiptResume
(
	Id					Int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Merchandising_GoodsReceiptResume PRIMARY KEY CLUSTERED,
	ReferenceNumberCsl	int NOT NULL CONSTRAINT FK_Collections_GoodsReceiptResume_GoodsReceipt FOREIGN KEY REFERENCES Merchandising.GoodsReceipt(Id),
	DateReceived		date NOT NULL,
	LocationId			int NOT NULL CONSTRAINT FK_Collections_GoodsReceiptResume_Location FOREIGN KEY REFERENCES Merchandising.Location(Id),
	Date				date NOT NULL,
	Quantity			int NOT NULL,
	LastLandedCost		decimal(19, 4) NULL,
	ProductId			int NOT NULL CONSTRAINT FK_Collections_GoodsReceiptResume_Product FOREIGN KEY REFERENCES Merchandising.Product(Id),
	VendorId			int NOT NULL CONSTRAINT FK_Collections_GoodsReceiptResume_supplier FOREIGN KEY REFERENCES Merchandising.supplier(Id),
	VendorName			VarChar(100) NOT NULL,
	PurchaseOrderId		int NOT NULL CONSTRAINT FK_Collections_GoodsReceiptResume_PurchaseOrder FOREIGN KEY REFERENCES Merchandising.PurchaseOrder(Id),
	SKU					varchar(20) NOT NULL,
	Description			varchar(240) NOT NULL,
	Status				VarChar(20),
	ReceivingLocationId int NOT NULL CONSTRAINT FK_Collections_ReceivingLocationId_Location FOREIGN KEY REFERENCES Merchandising.Location(Id),
	QuantityCancelled	Int NULL,
	QuantityOrdered		Int NULL,
	ReceiptProductId	Int NOT NULL
) 
GO

INSERT INTO Merchandising.GoodsReceiptResume
	(ReferenceNumberCsl, DateReceived, LocationId, Date, Quantity, LastLandedCost, ProductId, VendorId, VendorName, PurchaseOrderId, SKU, 
	Description, Status, ReceivingLocationId, QuantityCancelled, QuantityOrdered, ReceiptProductId)
SELECT 
	gr.id as ReferenceNumberCsl,	
	gr.DateReceived,
	gr.locationid LocationId,
	gr.datereceived as [Date],
	gr.quantityreceived AS Quantity,	
	gr.LastLandedCost,
	po.productid ProductId,
	po.VendorId,
	po.VendorName,
	po.PurchaseOrderId,	
	po.sku SKU,
	po.longdescription [Description],
	po.Status, 
	po.ReceivingLocationId,
	gr.QuantityCancelled,
	gr.QuantityReceived,
	gr.ReceiptProductId
FROM 
(
	SELECT 
		gr.id, 
		gr.datereceived, 
		grp.quantityreceived, 
		grp.lastlandedcost, 
		gr.locationid, 
		grp.purchaseorderproductid,
		grp.quantitycancelled, 
		grp.Id AS ReceiptProductId
	FROM 
		merchandising.goodsreceipt gr
		INNER JOIN merchandising.goodsreceiptproduct grp  ON grp.goodsreceiptid = gr.id
	WHERE
		grp.quantityreceived != 0
) GR
INNER JOIN 
(
	SELECT 
		pop.Id,
		po.vendorid,
		po.Vendor AS VendorName,
		pop.productid,
		pop.purchaseorderid,
		pop.quantityordered,
		pop.quantitycancelled,
		po.receivinglocationid,
		po.[status],
		p.SKU,
		p.LongDescription 
	FROM 
		merchandising.purchaseorderproduct pop 
		INNER JOIN merchandising.purchaseorder po ON pop.purchaseorderid = po.id
		JOIN merchandising.product p ON p.id = pop.productid AND p.producttype = 'RegularStock'
) po
	ON gr.purchaseorderproductid = po.id
GO

CREATE NONCLUSTERED INDEX IX_Merchandising_GoodsReceiptResume
ON Merchandising.GoodsReceiptResume
(
	ProductId, ReceivingLocationId, Status
)
INCLUDE 
(
	DateReceived, Quantity, QuantityCancelled, QuantityOrdered
)
