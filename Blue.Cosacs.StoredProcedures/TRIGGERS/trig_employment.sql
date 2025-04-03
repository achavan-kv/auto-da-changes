
if exists (select * from sysobjects where type='TR'
		and name = 'trig_employment')

drop trigger trig_employment
go


create trigger trig_employment
on employment
for update, insert
as declare @custid char(12), @error varchar (128),@worktype varchar (4),@payfreq varchar (4),
@empmtstatus varchar (4),@reason varchar (4),@code varchar (4)
/* prevent accounts with differences between agreement and employment being saved 
allowing up to one instalment difference*/
select top 1 @custid= custid ,@worktype=worktype,
@payfreq = payfreq,@empmtstatus= empmtstatus 
from inserted 

if @worktype !='' and @worktype is not null 
begin
   select @code = code from code where category like 'WT%' 
   and code =@worktype
   IF @@ROWcount = 0
   begin
      set @error = 'employment type (WT%) does not much code ' + @custid
      RAISERROR (@error, 16, 1)   
  end
End
if @payfreq !='' and @payfreq is not null 
begin
   select @code = code from code where category like 'PF%' 
   and code =@payfreq
   IF @@ROWcount = 0
   begin
      set @error = 'pay frequency (PF%) does not much code ' + @custid
      RAISERROR (@error, 16, 1)   
  end
End
if @empmtstatus !='' and @empmtstatus is not null 
begin
   select @code = code from code where category in ('ES1', 'ES2')
   and code =@empmtstatus
   IF @@ROWcount = 0
   begin
      set @error = 'employment status (ES%) does not match code ' + @custid
      RAISERROR (@error, 16, 1)   
  end
End

go