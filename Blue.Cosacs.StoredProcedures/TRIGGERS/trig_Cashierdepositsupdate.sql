
if  exists (select * from sysobjects where name = 'trig_Cashierdepositsupdate')

drop trigger trig_Cashierdepositsupdate
go


CREATE trigger trig_Cashierdepositsupdate on cashierdeposits
for update
as declare @empeeno integer,@depositvalue money,@branchno smallint,@paymethod varchar (4), @voided char(1),
@oldvoided  char(1), @runno int

select @empeeno = empeeno, @branchno = branchno,@depositvalue= depositvalue,@paymethod=paymethod,
@voided =voided, @runno = runno
from inserted 

select @oldvoided =voided from deleted

if @voided ='Y' and @oldvoided !='Y'
begin
	IF(@runno!=-1)		/*exclude deposits not going to FACT */
	BEGIN
		  update cashieroutstanding set depositoutstanding=depositoutstanding + @depositvalue 
		  where empeeno =@empeeno and paymethod =@paymethod and branchno =@branchno
		  if @@rowcount = 0 and @@error = 0
		  begin
		
		    insert into cashieroutstanding(empeeno, depositoutstanding , paymethod, branchno)
		          values (@empeeno,+@depositvalue,@paymethod,@branchno)
		  end
	END
end