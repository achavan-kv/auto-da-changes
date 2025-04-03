-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will arse default to 'true'.
-- 
-- Put your SQL code here

create table Merchandising.RetailPrice (
	Id int not null identity(1,1),
	
	LocationId int not null,
	ProductId int not null,
	EffectiveDate date not null,

	IncludesTax bit not null,
	RegularPrice money not null,
	CashPrice money not null,
	DutyFreePrice money not null,

	CreatedDate DateTime not null,
	CreatedById int not null,

	constraint PK_RetailPrice primary key clustered (id asc)
)

alter table Merchandising.RetailPrice
with check add constraint FK_Merchandising_RetailPrice_Location
foreign key (LocationId) references Merchandising.Location(Id)

alter table Merchandising.RetailPrice
with check add constraint FK_Merchandising_RetailPrice_Product
foreign key (ProductId) references Merchandising.Product(Id)

create unique nonclustered index [IX_RetailPrice] on Merchandising.RetailPrice (LocationId, ProductId, EffectiveDate)
