SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceValueGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceValueGetSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceValueGetSP
			@interface varchar(10),
			@runno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	interface, 
			runno, 
			counttype1, 
			counttype2, 
			branchno, 
			accttype, 
			countvalue, 
			value
	FROM		interfacevalue
	WHERE	interface = @interface
	AND 		runno = @runno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

