create table Merchandising.[StockCount] (
	Id int not null identity(1,1),
	LocationId int not null,
	[Type] char(10) not null,
	CountDate date not null,
	CreatedById int not null,
	CreatedDate date not null,
	constraint [PK_Merchandising_StockCount] primary key clustered (Id asc)
)

alter table Merchandising.[StockCount]
with check add constraint FK_Merchandising_StockCount_Location
foreign key (LocationId)
references Merchandising.Location(Id)
