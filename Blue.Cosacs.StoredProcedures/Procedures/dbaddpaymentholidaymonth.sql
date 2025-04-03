if  exists (select * from sysobjects  where name =  'dbaddpaymentholidaymonth' )
drop procedure dbaddpaymentholidaymonth
go
-- Procedure to add month to datefirst for all the account on anniversy of the account for paymentholiday
-- and to populate data in paymentholidays table to indicate that paymentholiday is taken
create procedure dbaddpaymentholidaymonth as
declare
@status integer, @statement  varchar (500)
set @statement = 'initial insert into temp table '

select 
       	convert (char(12),a.acctno) as acctno, 
       	g.datedel,
       	a.dateacctopen,
       	convert(smallint,0) as paymentholidaystaken, 
       	convert (smallint, 0) as numberPHdue
into 	#paymentholidays
from 	acct a, termstypetable ,agreement g 
where 	a.termstype =termstypetable.termstype
and 	g.PaymentHolidays>0 -- use that stored on the agreement rather than termstype which may have changed.
and 	a.acctno =g.acctno
and 	g.agrmtno =1 
and 	a.outstbal > 0
set @status = @@ERROR

if @status =0
begin
   create clustered index Ix_phder on #paymentholidays(acctno)
   set @status = @@error
end
	

if @status =0
begin
   set @statement ='updating paymentholidaystaken'
   update #paymentholidays
   set 	  paymentholidaystaken = (select isnull(count(*),0) from PaymentHolidays p
   				  where  p.acctno =#paymentholidays.acctno)
   set @status = @@error
end


if @status =0
begin
   set @statement ='updating numberPHdue'
   update #paymentholidays
   set    numberPHdue = datediff(day,dateacctopen,getdate())/365  -- RD/AA 67847 Changed from Year to day
   set @status = @@error
end

if @status=0
begin
   set @statement ='updating numberPHdue to be equal to total p holidays if > than'
   update #paymentholidays
   set numberPHdue = a.paymentholidays
   from agreement a where a.acctno = #paymentholidays.acctno
   and numberPHdue >a.paymentholidays --IP - 28/04/08 - UAT(318) - was originally commented out incorrectly
   set @status = @@error
end


-- removing from #payment holidays where datefirst already manually moved forward 72010
	DELETE p 
	FROM #paymentholidays P
	WHERE  EXISTS (
	SELECT * FROM agreement g 
	join instalplan i ON g.acctno = i.acctno
	WHERE  p.acctno = g.acctno
	AND DATEDIFF(day,  g.datedel, DATEADD(DAY,-20,i.DATEFIRST)) > (numberPHdue + 1)* 30.33
	and numberPHdue>paymentholidaystaken)


if @status=0
begin
   set @statement ='updating datefirst'
   update instalplan set datefirst =dateadd(month,1,datefirst), empeenochange=99999
	,datelast =dateadd(month,1,datelast) --AA reinstating as it should still show correct figure
   from #paymentholidays
   where #paymentholidays.acctno = instalplan.acctno and numberPHdue>paymentholidaystaken
set @status = @@error
end


if @status =0
begin
   set @statement ='final insert'
  insert into paymentholidays
   	(acctno ,
	 agrmtno ,
     	 datetaken,
     	 empeeno ,
   	 newdatefirst)
  select instalplan.acctno,
         instalplan.agrmtno,
         getdate(),
	 99999,
         datefirst
  from   #paymentholidays,instalplan
  where  #paymentholidays.acctno = instalplan.acctno and numberPHdue>paymentholidaystaken and instalplan.agrmtno=1
  set @status = @@error
end
if @status !=0
	print @statement
return @status

go
