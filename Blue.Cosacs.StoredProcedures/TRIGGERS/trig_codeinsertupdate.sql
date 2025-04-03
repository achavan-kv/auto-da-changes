if exists (select * from sysobjects where name = 'trig_codeinsert')
drop trigger trig_codeinsert
go
/*create trigger trig_codeinsert  --no longer required as everyone doing negative deposits now
on code for insert,update
as declare @code varchar(12) ,@category varchar(13), @reference varchar(12),@countrycode varchar (3),@error varchar (300)
select @code = code,@category = category ,@reference = reference from inserted

if @category ='FPM'
begin
	select @countrycode = countrycode from country
	IF @reference = '1' AND @countrycode !='S' and @countrycode !='P' and @countrycode !='Z'
   begin
      set @error =' Negative deposits only allowed for Singapore Code:' + @code + ' Category:' +  @category + '(raised by trig_codeinsert) '
  		RAISERROR(@error, 16, 1) 
  end
end*/

