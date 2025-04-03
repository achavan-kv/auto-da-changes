
if  exists (select * from sysobjects where name = 'trig_CourtsPerson_income')

drop trigger trig_CourtsPerson_income
go


create trigger trig_CourtsPerson_income on courtspersonTable
for update
as 
declare @datelstaudit DateTime, 
@datethis DateTime, 
@UserId integer

select @datelstaudit = datelstaudit, @UserId = UserId 
from deleted

select @datethis =datelstaudit  from inserted

if @datethis !=@datelstaudit 
begin

  delete from cashiertotalsincome 
  where datetrans < dateadd (day, - 30, getdate())
  and empeeno =@UserId
  
  insert into cashiertotalsincome 
  (branchno, acctno, transrefno, datetrans, 
	transtypecode, empeeno, transvalue, bankcode, 
	bankacctno, chequeno, paymethod )
   select branchno, acctno, transrefno, datetrans, 
   transtypecode, empeeno, transvalue, bankcode, 
   bankacctno, chequeno, paymethod 
   from fintrans_new_income 
   where empeeno =@UserId 
   and not exists (select * from 
   cashiertotalsincome c
   where c.acctno = fintrans_new_income.acctno and c.transrefno =fintrans_new_income.transrefno
   and fintrans_new_income.datetrans =c.datetrans)
	delete from fintrans_new_income where empeeno = @UserId and datetrans <= @datethis
end
go