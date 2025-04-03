SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetActivities]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetActivities]
GO

CREATE PROCEDURE 	dbo.DN_AccountGetActivities
			@acctno varchar(12),
			@serverdbname varchar(50),
			@isactivities smallint,
			@return int OUTPUT

AS

	DECLARE	@statement SQLText

	SET 	@return = 0			--initialise return code

	SET @statement = 'SELECT Activity_ID, Activity_Name, User_id, Date, InstallmentAmount,' +
			 'InstallmentDueDate, InstallmentPaidAmount ' +
		     	 'FROM ' + @serverdbname + '.dbo.TM_GeneralActivities ' +
		     	 'WHERE Account_Number = ' + @acctno

	IF(@isactivities = 1)
	BEGIN
		SET @statement = @statement + ' AND Activity_ID NOT IN (3,4,5,6,41) ORDER BY Date DESC'
	END
	ELSE
	BEGIN
		SET @statement = @statement + ' AND Activity_ID IN (3,4,5,6,41) ORDER BY Date DESC'
	END

	EXECUTE sp_executesql @statement

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_WARNINGS OFF 
GO

