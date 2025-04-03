
if exists (select * from sysobjects where type='TR'
		and name = 'trig_proposal')

drop trigger trig_proposal
go

create trigger trig_proposal
on proposal
for update, insert
as declare @acctno char(12), @error varchar (128),@proofaddress varchar (4),@proofincome varchar (4),
@proofid varchar (4),@reason varchar (4),@code varchar (4),@accttype char(1), @custid varchar (20),@dateprop datetime,
@appnumber integer,@oldappno integer , @points integer,@oldpoints integer
/* prevent accounts with differences between agreement and proposal being saved 
allowing up to one instalment difference*/
select top 1 @acctno= acctno ,@proofaddress=proofaddress,
@proofincome = proofincome,@proofiD= proofid ,@reason = reason, 
@custid = custid,@dateprop = dateprop,@oldappno =appnumber, @points = points
from inserted 

select top 1 @oldpoints = points
from deleted

if @proofaddress !='' and @proofaddress is not null 
begin
   select @code = code from code where category ='PAD' 
   and code =@proofaddress
   IF @@ROWcount = 0
   begin
      set @error = 'proof address does not much code ' + @acctno
      RAISERROR (@error, 16, 1)   
  end
End
if @proofincome !='' and @proofincome is not null 
begin
   select @code = code from code where category IN ('PIN','PIE','PIS') 
   and code =@proofincome
   IF @@ROWcount = 0
   begin
      set @error = 'proof income does not match code ' + @acctno
      RAISERROR (@error, 16, 1)   
  end
End
if @proofid !='' and @proofid is not null 
begin
   select @code = code from code where category ='PID' 
   and code =@proofid
   IF @@ROWcount = 0
   begin
      set @error = 'proof ID (PID) does not match code ' + @acctno
      RAISERROR (@error, 16, 1)   
  end
End
if @reason !='' and @reason is not null 
begin
   select @code = code from code where category ='SN1' 
   and code =@reason
   IF @@ROWcount = 0
   begin
      set @error = 'referral reason (SN1) does not match reason code ' + @acctno
      RAISERROR (@error, 16, 1)   
  end
End
--- incrementing app number each time an a/c is scored rf sub agreements allocated same rf app numbers as previous rf application.
set @appnumber = 0
select @accttype =accttype from acct where acctno =@acctno
begin
  if exists (select * from customer
         where custid =@custid and 
			(   datelastscored > dateadd (minute, -2, getdate())) -- recently scored a/c
			    AND ( @points !=@oldpoints AND @points <>0 ) --@points not equalling 0 should rule out rf sub agreements getting new appnumber
          ) 
  begin -- rescored, increment app number
  if @accttype='R'  --
      delete from Scorexappnumber where custid =@custid
  else
		delete from Scorexappnumber where acctno =@acctno
  
   insert into Scorexappnumber (acctno, custid,accttype, dateprop)
     values (@acctno,@custid,@accttype,@dateprop)
   select @appnumber = @@identity

   if @accttype !='R'  --
 	  update proposal set appnumber = @appnumber where 
		custid =@custid and 
      acctno =@acctno and 
		dateprop =@dateprop

  end
end

if @appnumber = 0 and @accttype ='R'
	select @appNumber = appnumber from Scorexappnumber where custid =@custid and accttype ='R'

if @accttype ='R' and @oldappno !=@appnumber --prevent loop in trigger
 	  update proposal set appnumber = @appnumber where 
		custid =@custid and 
      acctno =@acctno and 
		dateprop =@dateprop


go