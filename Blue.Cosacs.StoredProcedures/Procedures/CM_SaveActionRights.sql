SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveActionRights]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveActionRights]
GO

-- =============================================
-- Author:		Alex Ayscough
-- Create date: 08/01/2009
-- Description:	Saves action rights 
-- =============================================

CREATE PROCEDURE [dbo].[CM_SaveActionRights] 
    @EmpeeNo INT,
	@Strategy VARCHAR(10),
	@Action VARCHAR(10),
	@Empeenochange INT,
	@EmpeeType VARCHAR(100),
	@CycleToNextFlag BIT,
	@MinNotesLength INT,
    @return INT OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
    
    --IP - 28/09/10 - UAT(37) UAT5.4 - Update MinNotesLength for the action being inserted/updated.
    SELECT @MinNotesLength = reference FROM code WHERE category = 'FUA' AND code = @Action
	
	UPDATE CMActionRights
	SET 
	Empeenochange=  @Empeenochange,
	CycleToNextFlag=  @CycleToNextFlag,
	MinNotesLength=  @MinNotesLength
	WHERE EmpeeNo=  @EmpeeNo AND 
	Strategy=  @Strategy AND  
	Action=  @Action 

	IF @@ROWCOUNT = 0

    INSERT INTO CMActionRights (
		EmpeeNo,
		Strategy,
		[Action],
		Empeenochange,
		CycleToNextFlag,
		MinNotesLength
	) VALUES ( 
		@EmpeeNo,
		@Strategy,
		@Action,
		@Empeenochange,
		@CycleToNextFlag,
		@MinNotesLength ) 

	    SET @return = @@ERROR

	IF @empeeno = 0  -- then this needs to insert rights for all user types
	BEGIN
		INSERT INTO CMActionRights (
			EmpeeNo,
			Strategy,
			[Action],
			Empeenochange,
			CycleToNextFlag,
			MinNotesLength
			)	
			SELECT 
			ur.UserId,
			@strategy,
			@action,
			@empeenochange,
			@cycletoNextFlag,
			@MinnotesLength
		FROM Admin.[UserRole] ur
        INNER JOIN Admin.Role r ON ur.RoleId = r.Id 
		WHERE r.Name = @empeetype
		AND NOT EXISTS (SELECT * FROM CMActionRights R 
						WHERE r.EmpeeNo=  ur.UserId
						AND r.Strategy=  @Strategy 
						AND r.Action=  @Action)
	END 	


	RETURN @return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

