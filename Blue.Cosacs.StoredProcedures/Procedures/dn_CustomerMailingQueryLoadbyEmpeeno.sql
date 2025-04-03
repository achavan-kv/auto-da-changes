if exists (select * from sysobjects where name ='dn_CustomerMailingQueryLoadbyEmpeeno')
drop procedure dn_CustomerMailingQueryLoadbyEmpeeno
go
create procedure dn_CustomerMailingQueryLoadbyEmpeeno
   @EmpeenoSave   int ,
   @return INT out
as
    set @return = 0
    select QueryName,
           datesaved
    from CustomerMailingQuery
    where
    EmpeenoSave  = @EmpeenoSave
    order by datesaved desc -- want most recent at the top
     return @return

go

