if exists (select * from sysobjects where name ='DN_InterfaceErrorRemoveOldSP')
drop procedure DN_InterfaceErrorRemoveOldSP
go
create procedure DN_InterfaceErrorRemoveOldSP 
@interface varchar(12),
@runno int,
@return int OUT
as 

set @return = 0

delete from interfaceerror
where runno = @runno
and interface = @interface

set @return = @@error

return  @return

go