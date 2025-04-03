if not exists(select * from sys.columns 
            where Name = N'CurrentLocationId' and Object_ID = Object_ID(N'merchandising.StockTransferProduct'))
begin
	delete from Merchandising.StockTransferProduct

	alter table Merchandising.StockTransferProduct
	add CurrentLocationId int not null

	alter table Merchandising.StockTransferProduct
	add constraint FK_Merchandising_StockTransferProduct_CurrentLocationId foreign key (CurrentLocationId) references Merchandising.Location(Id)
end