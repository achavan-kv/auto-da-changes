alter table Merchandising.StockCount
add StartedById int null

alter table Merchandising.StockCount
add constraint FK_Merchandising_StockCount_StartedBy
foreign key(StartedById) references [Admin].[User](Id)