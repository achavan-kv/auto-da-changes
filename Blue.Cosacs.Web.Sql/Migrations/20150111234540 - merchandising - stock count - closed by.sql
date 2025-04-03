alter table Merchandising.StockCount
add ClosedById int null
   ,ClosedDate datetime null

alter table Merchandising.StockCount
add constraint FK_Merchandising_StockCount_ClosedBy
foreign key(ClosedById) references [Admin].[User](Id)

alter table Merchandising.StockCount
alter column CreatedDate datetime not null

alter table Merchandising.StockCount
alter column CancelledDate datetime null
