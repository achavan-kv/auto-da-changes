SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployeeTerminateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployeeTerminateSP]
GO

CREATE PROCEDURE 	dbo.DN_EmployeeTerminateSP
-- =============================================
-- Author:??
-- Create date: ??
-- Title:	DN_EmployeeTerminateSP
--
--	This procedure will Terminate and employee
-- 
-- Change Control
-----------------
-- 10/01/12 jec #9360 LW74502 - Account allocating to fired Bailiff
-- =============================================
			@empeeno int,
			@empeechange int,
			@return int OUTPUT

AS

RAISERROR ('This stored procedure should never be used.',16,1)

--	SET 	@return = 0			--initialise return code

--	UPDATE	courtsperson
--	SET empeechange = @empeechange, MinAccounts=0,MaxAccounts=0		-- #9360
--	WHERE	userid = @empeeno
	
--	DELETE FROM Admin.UserRole
--	WHERE UserId = @empeeno
	
--	-- #9360 If Employee has left the company (empeetype=Z) set Min/Max accounts to 0 and set Reallocate to 1 on CMBailiffAllocationRules (if employee exists)
			
--	UPDATE CMBailiffAllocationRules
--	set Reallocate=1, empeenochange=@empeechange,DateChange=GETDATE()
--	Where empeeno=@empeeno			

--	SET @return = @@error
--GO
--SET QUOTED_IDENTIFIER OFF 
--GO
--SET ANSI_NULLS ON 
--GO

---- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
 