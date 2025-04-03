if  exists (select * from sysobjects  where name =  'trig_code_sundry' )
drop trigger trig_code_sundry
go
-- removing as this code now in the nomral trigger trig_code_update
-- sometimes only one trigger executed