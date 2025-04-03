SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceValueAddSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceValueAddSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceValueAddSP
			@interface varchar(10),
			@runno int,
			@counttype1 varchar(25),
			@counttype2 varchar(10),
			@branchno int,
			@accttype varchar(10),
			@countvalue int,
			@value money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT INTO interfacevalue(interface, runno, counttype1, counttype2, branchno, accttype, countvalue, value)
	VALUES(@interface, @runno, @counttype1, @counttype2, @branchno, @accttype, @countvalue, @value)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

