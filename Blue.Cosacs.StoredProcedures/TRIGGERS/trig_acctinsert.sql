if exists ( select * from sysobjects where name = 'trig_acctinsert')

drop trigger trig_acctinsert
go

create trigger trig_acctinsert
on acct
for insert as declare @acctno char(12),@accttype varchar (2),@agreementtotal money,
@error varchar (256)
select  @acctno = acctno,
        @accttype =accttype,
        @agreementtotal =agrmttotal from inserted

if @agreementtotal < -.01
begin
         rollback
         set @error =' agreement total should not be < 0 for acctno ' + @acctno + ' raised by trigger trig_acctinsert'
  		   RAISERROR(@error, 16, 1) 
end
   if @accttype = 'R'
   BEGIN
      EXECUTE dbnewauth @acctno = @acctno
   END
go