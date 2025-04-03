create table Merchandising.[StockAdjustmentProduct] (
	Id int not null identity(1,1),	
	StockAdjustmentId int not null,
	ProductId int not null,
	Quantity int not null,
	Comments varchar(max) null,
	constraint [PK_Merchandising_StockAdjustmentProduct] primary key clustered (Id asc)
)

alter table Merchandising.[StockAdjustmentProduct]
with check add constraint FK_Merchandising_StockAdjustmentProduct_StockAdjustment
foreign key (StockAdjustmentId)
references Merchandising.StockAdjustment(Id)

alter table Merchandising.[StockAdjustmentProduct]
with check add constraint FK_Merchandising_StockAdjustmentProduct_Product
foreign key (ProductId)
references Merchandising.Product(Id)

create unique nonclustered index [IX_StockAdjustmentProduct] on [Merchandising].[StockAdjustmentProduct] (
	StockAdjustmentId asc,
	ProductId asc
)