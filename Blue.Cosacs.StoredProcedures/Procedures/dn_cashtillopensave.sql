SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_cashtillopensave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_cashtillopensave]
GO

CREATE procedure dn_cashtillopensave @user integer,
	@reasoncode varchar (4),
             @tillid varchar (16),
	@return int OUTPUT
as 

   set @return = 0
   set nocount on
   
   delete from cashtillopen where empeeno =@user and timeopen < dateadd (month, - 3, getdate())
   set @return =@@error
   if @return = 0
   begin
      insert into cashtillopen
      ( empeeno,
        timeopen, 
        reasoncode,
        tillid
      )

     values ( @user,
       getdate(),
       @reasoncode,
        @tillid)
      set @return = @@error
   end

   return @return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

