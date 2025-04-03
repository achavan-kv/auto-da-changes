if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeHistorySaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
    drop procedure [dbo].[DN_TermsTypeHistorySaveSP]
go

create procedure DN_TermsTypeHistorySaveSP

@termstype varchar(2),
@empeenochange int,
@changedfield varchar(64),
@origvalue varchar(400),
@newvalue varchar(400)
as
   insert into TermsTypeHistory
       (datechange, termstype,empeenochange,changedfield,origvalue,newvalue)
   values
       (GETDATE(), @termstype,@empeenochange,@changedfield,@origvalue,@newvalue)

GO
