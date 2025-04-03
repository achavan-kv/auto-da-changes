SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SaveScoreDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SaveScoreDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_SaveScoreDetailsSP
			@custid varchar(20),
			@acctno varchar(12),
			@datescored datetime,
			@operandname varchar(150),
			@operandvalue varchar(50),
			@points int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT	
	INTO	ScoringDetails 
		(custid, acctno, datescored, operandname, operandvalue, points)
	VALUES	(@custid, @acctno, @datescored, @operandname, @operandvalue, @points)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

