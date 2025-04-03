SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PreExistDelCheckSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PreExistDelCheckSP]
GO

CREATE PROCEDURE 	dbo.DN_PreExistDelCheckSP
			@acctno varchar(12),
			@exists smallint OUTPUT,
			@return int OUTPUT

AS

	DECLARE @num_rows int

	SET 	@return = 0			--initialise return code
	SET @exists = 0

	SELECT	@num_rows = COUNT(*)
	FROM 		lineitem li, delivery d
	WHERE 	li.acctno = @acctno
	AND 		li.acctno = d.acctno
	AND 		li.itemno = d.itemno

	IF @num_rows > 0
	BEGIN
		SET @exists = 1
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

