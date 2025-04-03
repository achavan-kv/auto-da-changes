if exists (select * FROM sysobjects where  name ='dn_StampFintransRunno')
drop procedure dn_StampFintransRunno 
go
create procedure dn_StampFintransRunno @runno int, @return int out
as
set nocount on
set @return = 0
	update fintrans set runno =@runno where runno = 0 and exists (
	select * from 
	fintransLastSummary S
   where S.acctno = fintrans.acctno and S.datetrans = fintrans.datetrans
   and S.transrefno = fintrans.transrefno and S.branchno = fintrans.branchno)

	update cashierdeposits set runno =@runno where runno = 0
        and exists (select * from DepositssrecentInterfaced d
        where d.runno = @runno and d.depositid = depositid
        and d.datedeposit = cashierdeposits.datedeposit)
   SET @return =@@error
   return @return
--GO

select * from  countrymaintenance cm where 
--countrymaintenance 
cm.Name
like 'fact200%'
select * from  interfaceerror e where e.errordate >'1-may-2007'
and e.ErrorText  like 'fact%'
order by  e.errordate desc