create table Merchandising.GoodsReceipt (
	Id int NOT NULL IDENTITY(1,1),
	
	LocationId int NOT NULL,
	ReceivedById int NOT NULL,
	ProcessedById int NULL,
	Status varchar(100) NULL,
	
	VendorDeliveryNumber varchar(100) NULL,
	VendorInvoiceNumber varchar(100) NULL,
	DateReceived Date NOT NULL,
	Comments varchar(max) NULL,

	CONSTRAINT [PK_Merchandising_GoodsReceipt] PRIMARY KEY CLUSTERED (Id ASC),
)

alter table Merchandising.GoodsReceipt with check add constraint FK_Merchandising_GoodsReceipt_Location foreign key (LocationId) references Merchandising.Location(Id)
alter table Merchandising.GoodsReceipt with check add constraint FK_Merchandising_GoodsReceipt_ReceivedBy foreign key (ReceivedById) references [Admin].[User](Id)
alter table Merchandising.GoodsReceipt with check add constraint FK_Merchandising_GoodsReceipt_ProcessedBy foreign key (ProcessedById) references [Admin].[User](Id)