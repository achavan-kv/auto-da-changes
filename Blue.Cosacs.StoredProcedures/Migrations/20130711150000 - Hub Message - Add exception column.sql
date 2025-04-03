if not exists(select * from sys.columns 
            where Name = N'Exception' and Object_ID = Object_ID(N'Hub.Message'))    
begin
	alter table Hub.Message
	add Exception nvarchar(max)
end

