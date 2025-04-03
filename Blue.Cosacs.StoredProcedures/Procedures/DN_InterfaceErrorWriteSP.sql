SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceErrorWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceErrorWriteSP]
GO



CREATE PROCEDURE 	dbo.DN_InterfaceErrorWriteSP
			@interface varchar(20),
			@runno int,
			@errorDate datetime,
			@errorText ntext,
			@severity char(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	IF @runno= 0 -- select the latest run number from interface control
		SELECT @runno=isnull(MAX(runno),0)
		FROM interfacecontrol 
		WHERE interface = @interface
		
	INSERT
	INTO		interfaceerror
			(interface, runno, errordate, errortext, severity)
	VALUES	(@interface, @runno, @errorDate, convert(varchar(1000),@errorText), @severity)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

