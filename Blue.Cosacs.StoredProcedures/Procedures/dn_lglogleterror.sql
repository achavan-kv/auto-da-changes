SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_lglogleterror]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_lglogleterror]
GO

CREATE procedure dn_lglogleterror @runno smallint,
                                  @errortext varchar (2000),
                                  @lettercode varchar (10)
as declare
    @status  integer
begin
set @status = 1
        set @errortext = 'Letter ' + @lettercode + ' generation failure ' + @errortext
        insert into interfaceerror (interface,
         runno,
         errordate,
         severity,
         ErrorText)
       values

        ('LetterGen',
        @runno ,
         getdate() ,
        'W',
         @errortext) 
        return @status       
   
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

