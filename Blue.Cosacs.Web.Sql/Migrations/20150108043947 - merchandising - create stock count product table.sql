

create table Merchandising.[StockCountProduct] (
	Id int not null identity(1,1),
	StockCountId int not null,
	ProductId int NOT NULL,
	StartStockOnHand int NULL,
	[Count] int NULL,
	SystemAdjustment int NULL,
	Variance int NULL,
	NetMovement int NULL,
	CurrentStockOnHand int NULL,
	CreatedById int NOT	NULL,
	CreatedDate date NOT NULL,
	constraint [PK_Merchandising_StockCountProduct] primary key clustered (Id asc)
)

alter table Merchandising.[StockCountProduct]
with check add constraint FK_Merchandising_StockCountProduct_StockCount
foreign key (StockCountId)
references Merchandising.StockCount(Id)

alter table Merchandising.[StockCountProduct]
with check add constraint FK_Merchandising_StockCountProduct_Product
foreign key (ProductId)
references Merchandising.Product(Id)