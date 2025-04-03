
if  exists (select * from sysobjects where name = 'trig_Cashierstbreakdown')

drop trigger trig_Cashierstbreakdown
go


create trigger trig_Cashierstbreakdown on cashiertotalsbreakdown
for insert
as declare @empeeno integer,@value money,@branchno smallint,@paymethod varchar (4),@id integer
select @value= usertotal  + ISNULL(PettyCash,0)
 + ISNULL(Remittances,0)
 + ISNULL(Allocations,0)
 + ISNULL(Disbursements,0)

,@id =cashiertotalid,@paymethod =paymethod from inserted

select @branchno = branchno,@empeeno = empeeno from CashierTotals where id =@ID

update cashieroutstanding set depositoutstanding=depositoutstanding + @value 
where empeeno =@empeeno and paymethod =@paymethod and branchno =@branchno
if @@rowcount = 0 and @@error = 0 AND @paymethod !=13 /*exclude StoreCard*/
begin

 insert into cashieroutstanding(empeeno, depositoutstanding , paymethod, branchno)
          values (@empeeno,@value,@paymethod,@branchno)
end
go