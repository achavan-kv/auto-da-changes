
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetCallReminderInfoSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetCallReminderInfoSP]
GO

-- ============================================================================================
-- Author:		Ilyas Parker & Nasmi Mohamed
-- Create date: 02/01/2009
-- Description:	The procedure will return all call reminders for the selected account from 
--				the Telephone Action screen.	
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_GetCallReminderInfoSP] 
				@acctno varchar(12),
	            @empeeno int,
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;
	
	set @return = 0

	--Need to select the reminders that are due
	--for the selected account for the current employee.
	select * from cmreminder 
	where acctno = @acctno
	and	(empeeno = @empeeno or code = 'REM')
	and reminderDateTime < getdate()
	and status = 'N'
	and active = 1
	order by reminderdatetime desc

    if (@@error != 0)
    begin
        set @return = @@error
    end
end






GO
