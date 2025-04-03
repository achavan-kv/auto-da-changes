SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineitemAccountAddedToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineitemAccountAddedToSP]
GO

CREATE PROCEDURE 	dbo.DN_LineitemAccountAddedToSP
			@acctno varchar(12),
			@addto smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET	@addto = 0

	SELECT 	@addto = count(*)
	FROM		lineitem 
	WHERE	acctno = @acctno
	AND		itemno = 'ADDDR'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

