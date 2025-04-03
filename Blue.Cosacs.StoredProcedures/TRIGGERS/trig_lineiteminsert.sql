if exists (select * from sysobjects where name = 'trig_lineiteminsert')
drop trigger trig_lineiteminsert
go
create trigger trig_lineiteminsert
on lineitem for insert
as declare @acctno char(12) ,@itemno varchar (18),@stocklocn smallint,@contractno varchar (10),@quantity SMALLINT,
@error varchar(256), @category INTEGER, @itemId INT		-- RI jec 06/06/11

select @acctno = acctno,@itemno = IUPC,@itemId=i.itemId,		-- RI jec 06/06/11
	@stocklocn = stocklocn,@contractno= contractno,@quantity=quantity
from inserted i INNER JOIN Stockinfo s on i.itemId=s.ID		 

if @contractno =''
begin
	select @category = category from stockitem where itemId =@itemId and stocklocn =@stocklocn	-- RI jec 06/06/11
   if @category in (select distinct code from code where category = 'WAR') --warranty categories
   begin
      set @error =' error blank contract saving ' + @acctno + ' ' +  @itemno
  		RAISERROR(@error, 16, 1) 
  end
END


if @quantity >1
begin
	select @category = ISNULL(category,0) from stockitem where itemid =@itemId and stocklocn =@stocklocn	-- RI jec 06/06/11
   if @category in (select distinct code from code where category = 'WAR') --warranty categories
   begin
      set @error =' error quantity cannot be >1 saving ' + @acctno + ' ' +  @itemno
  		RAISERROR(@error, 16, 1) 
  end
end
go
