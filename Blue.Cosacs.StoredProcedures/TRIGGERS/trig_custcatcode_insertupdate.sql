/*AA 09/09/2004 Not doing this for runno not 0 */
if exists (select * from sysobjects where name = 'trig_custcatcode_insertupdate')
drop trigger trig_custcatcode_insertupdate 
go
create trigger trig_custcatcode_insertupdate on custcatcode
for update, insert
as declare @error varchar (500),@code varchar (6)
	iF exists (	
		select 	code from inserted, custacct,acct
		where custacct.custid = inserted.custid
		and custacct.acctno =acct.acctno and custacct.hldorjnt= 'H'
      and code ='STAF' and acct.securitised ='Y')
   begin
      set @error =' securitised accounts cannot be staff:' 
  		RAISERROR(@error, 16, 1) 
   end
go



