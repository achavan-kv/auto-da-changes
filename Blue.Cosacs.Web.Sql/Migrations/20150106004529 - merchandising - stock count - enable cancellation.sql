alter table Merchandising.StockCount
add CancelledDate date null
   ,CancelledById int null
   
alter table Merchandising.[StockCount]
with check add constraint FK_Merchandising_StockCount_CancelledBy
foreign key (CancelledById)
references [Admin].[User](Id)

alter table Merchandising.[StockCount]
with check add constraint FK_Merchandising_StockCount_CreatedBy
foreign key (CreatedById)
references [Admin].[User](Id)
