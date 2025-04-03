/*cr 673 securitised accounts cannot be staff*/
if exists (select * from sysobjects where name = 'trig_acctcode_insertupdate')
drop trigger trig_acctcode_insertupdate 
go
create trigger trig_acctcode_insertupdate on acctcode
for update, insert
as declare @error varchar (500),@code varchar (6)
	iF exists (	
		select 	code from inserted,acct
		where 
		 inserted.acctno =acct.acctno 
      and code ='STAF' and acct.securitised ='Y')
   begin
      set @error =' securitised accounts cannot be staff:' 
  		RAISERROR(@error, 16, 1) 
		rollback
   end
go



