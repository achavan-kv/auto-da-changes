if exists (select * from dbo.sysobjects where id = object_id('[dbo].[TermstypehistorysaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[TermstypehistorysaveSP]
go
create procedure TermstypehistorysaveSP 
@termstype varchar(2),
@empeenochange int, 
@changedfield varchar(64),
@origvalue varchar(400),
@newvalue varchar(400)
as
   insert into TermstypehistorysaveSP
   (termstype,empeenochange,changedfield,origvalue,newvalue)
 
   values
  (@termstype,@empeenochange,@changedfield,@origvalue,@newvalue)
 

go