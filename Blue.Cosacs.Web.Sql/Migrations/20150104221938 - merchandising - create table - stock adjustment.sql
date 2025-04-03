create table Merchandising.[StockAdjustment] (
	Id int not null identity(1,1),	
	LocationId int not null,
	PrimaryReasonId int not null,
	SecondaryReasonId int not null,
	Notes varchar(max) null,
	AuthorisedDate date null,
	AuthorisedById int null,
	constraint [PK_Merchandising_StockAdjustment] primary key clustered (Id asc)
)

alter table Merchandising.[StockAdjustment]
with check add constraint FK_Merchandising_StockAdjustment_Location
foreign key (LocationId)
references Merchandising.Location(Id)

alter table Merchandising.[StockAdjustment]
with check add constraint FK_Merchandising_StockAdjustment_PrimaryReason
foreign key (PrimaryReasonId)
references Merchandising.StockAdjustmentPrimaryReason(Id)

alter table Merchandising.[StockAdjustment] 
with check add constraint FK_Merchandising_StockAdjustment_SecondaryReason
foreign key (SecondaryReasonId)
references Merchandising.StockAdjustmentSecondaryReason(Id)

alter table Merchandising.[StockAdjustment]
with check add constraint FK_Merchandising_StockAdjustment_AuthorisedBy
foreign key (AuthorisedById)
references [Admin].[User](Id)