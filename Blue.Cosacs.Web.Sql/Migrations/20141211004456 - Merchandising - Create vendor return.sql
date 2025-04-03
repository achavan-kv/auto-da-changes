-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
create table Merchandising.VendorReturn (
	Id int not null identity(1,1)	
   ,GoodsReceiptId int not null
   ,CreatedById int null  
   ,ApprovedById int null 
   ,CreatedBy varchar(100) null
   ,ApprovedBy varchar(100) null 
   ,CreatedDate datetime not null
   ,ApprovedDate date null   
   ,Comments varchar(max) null
   ,constraint [PK_Merchandising_VendorReturn] primary key clustered (Id asc)
)

alter table Merchandising.VendorReturn with check add constraint FK_Merchandising_VendorReturn_GoodsReceipt foreign key (GoodsReceiptId) references [Merchandising].[GoodsReceipt](Id)
alter table Merchandising.VendorReturn with check add constraint FK_Merchandising_VendorReturn_CreatedBy foreign key (CreatedById) references [Admin].[User](Id)
alter table Merchandising.VendorReturn with check add constraint FK_Merchandising_VendorReturn_ApprovedBy foreign key (ApprovedById) references [Admin].[User](Id)


create table Merchandising.VendorReturnProduct (
	Id int not null identity(1,1),
	VendorReturnId int not null,
	GoodsReceiptProductId int not null,		
	QuantityReturned int not null,	
	Comments varchar(100) null,
	constraint [PK_Merchandising_VendorReturnProduct] primary key clustered (Id asc)
)

alter table Merchandising.VendorReturnProduct with check add constraint FK_Merchandising_VendorReturnProduct_VendorReturn foreign key (VendorReturnId) references Merchandising.VendorReturn(Id)
alter table Merchandising.VendorReturnProduct with check add constraint FK_Merchandising_VendorReturnProduct_GoodsReceiptProduct foreign key (GoodsReceiptProductId) references Merchandising.GoodsReceiptProduct(Id)
