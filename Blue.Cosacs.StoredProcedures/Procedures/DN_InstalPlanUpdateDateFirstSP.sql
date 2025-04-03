SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InstalPlanUpdateDateFirstSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InstalPlanUpdateDateFirstSP]
GO

CREATE PROCEDURE 	dbo.DN_InstalPlanUpdateDateFirstSP
			@acctno varchar(12),
			@datefirst datetime,
			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	UPDATE	instalplan
	SET		datefirst = @datefirst,
			datelast = dateadd(month, instalno - 1, @datefirst),
			dueday = cast(datepart(day, @datefirst) AS smallint), 
			empeenochange = @user
	WHERE	acctno = @acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

   --procedure for updating variable date of first instalments
    declare @counter smallint,@datenext datetime
    set @datenext =@datefirst
    set @counter = 1

    while 1 = 1 
    begin
       update instalmentvariable set datefrom = @datenext
       where acctno = @acctno 
       and instalorder =@counter
       if @@rowcount = 0 
          break

       select  @datenext= dateadd(month,1,dateto) --dateto is a calculated column from date from 
       from instalmentvariable 
       where acctno = @acctno 
       and instalorder =@counter 

       set @counter = @counter + 1

    end
 	 IF (@@error != 0)
 	 BEGIN
		SET @return = @@error
	 END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

