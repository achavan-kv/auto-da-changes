create table Merchandising.GoodsReceiptDirect (
	Id int not null identity(1,1)
	
   ,LocationId int not null
   ,CreatedById int null
   ,ReceivedById int not null
   ,ApprovedById int null
   ,VendorId int null

   ,CreatedBy varchar(100) null
   ,ReceivedBy varchar(100) null
   ,ApprovedBy varchar(100) null
   ,Vendor varchar(100) null
   ,Currency varchar(100) null

   ,CreatedDate datetime not null
   ,OriginalPrint datetime null

   ,DateReceived date not null
   ,DateApproved date null
   ,ReferenceNumbers varchar(max) null
   ,VendorDeliveryNumber varchar(100) null
   ,VendorInvoiceNumber varchar(100) null
   ,Comments varchar(max) null

   ,constraint [PK_Merchandising_GoodsReceiptDirect] primary key clustered (Id asc)
)

alter table Merchandising.GoodsReceiptDirect with check add constraint FK_Merchandising_GoodsReceiptDirect_Location foreign key (LocationId) references Merchandising.Location(Id)
alter table Merchandising.GoodsReceiptDirect with check add constraint FK_Merchandising_GoodsReceiptDirect_Vendor foreign key (VendorId) references [Merchandising].[Supplier](Id)
alter table Merchandising.GoodsReceiptDirect with check add constraint FK_Merchandising_GoodsReceiptDirect_CreatedBy foreign key (CreatedById) references [Admin].[User](Id)
alter table Merchandising.GoodsReceiptDirect with check add constraint FK_Merchandising_GoodsReceiptDirect_ReceivedBy foreign key (ReceivedById) references [Admin].[User](Id)
alter table Merchandising.GoodsReceiptDirect with check add constraint FK_Merchandising_GoodsReceiptDirect_ApprovedBy foreign key (ApprovedById) references [Admin].[User](Id)


create table Merchandising.GoodsReceiptDirectProduct (
	Id int not null identity(1,1),
	
	GoodsReceiptDirectId int not null,
	ProductId int not null,
	VendorId int not null,

	Sku varchar(100) null,
	[Description] varchar(100) null,
	QuantityReceived int not null,
	UnitLandedCost money not null,
	
	Comments varchar(100) null,

	constraint [PK_Merchandising_GoodsReceiptDirectProduct] primary key clustered (Id asc)
)

alter table Merchandising.GoodsReceiptDirectProduct with check add constraint FK_Merchandising_GoodsReceiptDirectProduct_GoodsReceiptDirect foreign key (GoodsReceiptDirectId) references Merchandising.GoodsReceiptDirect(Id)
alter table Merchandising.GoodsReceiptDirectProduct with check add constraint FK_Merchandising_GoodsReceiptDirectProduct_Product foreign key (ProductId) references Merchandising.Product(Id)
alter table Merchandising.GoodsReceiptDirectProduct with check add constraint FK_Merchandising_GoodsReceiptDirectProduct_Supplier foreign key (VendorId) references Merchandising.Supplier(Id)