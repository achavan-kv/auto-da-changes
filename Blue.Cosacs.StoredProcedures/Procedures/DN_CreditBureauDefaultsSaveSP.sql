SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CreditBureauDefaultsSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CreditBureauDefaultsSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_CreditBureauDefaultsSaveSP
			@custid varchar(20),
			@status varchar(8),
			@defaultsBalance money,
			@defaults smallint, 
			@defaultsExMotorBalance money,
			@defaultsExMotor smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	CreditBureauDefaults
	SET		defaultsbalance = @defaultsBalance,
			defaults = @defaults,	
			defaultsexmotorbalance = @defaultsExMotorBalance,
			defaultsexmotor = @defaultsExMotor
	WHERE	custid = @custid 
	AND		status = @status

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		creditbureaudefaults
				(custid, status, defaultsbalance, defaults,
				defaultsexmotorbalance, defaultsexmotor)
		VALUES	(@custid, @status, @defaultsBalance, @defaults,
				@defaultsExMotorBalance, @defaultsExMotor)
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

