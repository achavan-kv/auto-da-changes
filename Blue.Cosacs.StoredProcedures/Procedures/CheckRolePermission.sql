--SET QUOTED_IDENTIFIER OFF 
--GO
--SET ANSI_NULLS ON         -- We don't want (NULL = NULL) to be TRUE
--GO

---- Call this procedure only when you need to validate a user's permission from an external systems like EPOS etc. 

--IF EXISTS (SELECT * FROM SysObjects WHERE id = OBJECT_ID('[dbo].CheckRolePermission') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
--	DROP PROCEDURE dbo.CheckRolePermission
--GO

--CREATE PROCEDURE dbo.CheckRolePermission 
--(
--	@EmpeeNo INT,
--	@Password VARCHAR(12),
--	@TaskName VARCHAR(100),
--	@IsPermitted BIT OUT,
--	@Message VARCHAR(200) OUT,
--	@Return int OUTPUT		
--)

--AS
--BEGIN
--	SET NOCOUNT ON
--	SET @Return = 0
	
--	SET @IsPermitted = 0
--	SET @Message = ''
	
--	-- Verify Password -------------------------------------
--	IF NOT EXISTS(SELECT 1 FROM dbo.courtsperson WHERE empeeno = @EmpeeNo)
--		SET @Message = 'Invalid Credentials'
--	ELSE
--	BEGIN	
--		DECLARE @Index INT, @Sum INT, @Char CHAR(1)
--		SET @Index = 0
--		SET @Sum = 113
		
--		WHILE(@Index < LEN(@Password))
--		BEGIN
--			SET @Char = (RIGHT(LEFT(@password,@Index + 1),1))
--            SET @Sum = @Sum + ASCII(@Char) * (@Index + 1) 
            
--            SET @Index = @Index + 1
--		END
		
--		IF NOT EXISTS(SELECT 1 FROM dbo.courtsperson WHERE empeeno = @EmpeeNo AND password = CONVERT(VARCHAR, @Sum))
--			SET @Message = 'Invalid Credentials'		
--	END
--	--------------------------------------------------------
	
--	-- Verify Role Permission ------------------------------
--	IF(@Message = '' AND EXISTS(SELECT 1 FROM Admin.UserPermission
--								WHERE Name = @TaskName
--								AND UserId = @EmpeeNo))
--	BEGIN
--		SET @IsPermitted = 1
--	END


--	IF(@Message = '' AND @IsPermitted = 0)
--	BEGIN
--		SET @Message = 'Not Permitted'
--	END	
--	--------------------------------------------------------
	
--	SET @Return = @@error
	
--END	

--GO	



					