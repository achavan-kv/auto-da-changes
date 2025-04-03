
if  exists (select * from sysobjects where name = 'trig_Cashierdepositsinsert')

drop trigger trig_Cashierdepositsinsert
go



CREATE trigger trig_Cashierdepositsinsert on cashierdeposits
for insert
as declare @empeeno integer,@depositvalue money,@branchno smallint,@paymethod varchar (4), @voided char(1), @runno int

select @empeeno = empeeno, @branchno = branchno,@depositvalue= depositvalue,@paymethod=paymethod,
@voided =voided, @runno=runno
from inserted 
if @voided !='Y'
begin
  	IF(@runno!=-1)		/*exclude deposits not going to FACT */
	BEGIN
		update cashieroutstanding set depositoutstanding=depositoutstanding - @depositvalue 
		where empeeno =@empeeno and paymethod =@paymethod and branchno =@branchno
		if @@rowcount = 0 and @@error = 0
		begin
			insert into cashieroutstanding(empeeno, depositoutstanding , paymethod, branchno)
	          		values (@empeeno,-@depositvalue,@paymethod,@branchno)
	  	end
	END
end
