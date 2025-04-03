if not exists(select * from sys.columns 
            where Name = N'QuantityCancelled' and Object_ID = Object_ID(N'merchandising.stockallocationproduct'))
begin
    alter table merchandising.stockallocationproduct
	alter column QuantityCancelled int not null

	alter table merchandising.stockrequisitionproduct
	alter column QuantityCancelled int not null

	alter table merchandising.stocktransferproduct
	alter column QuantityCancelled int not null
end
