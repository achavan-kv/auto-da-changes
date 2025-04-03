SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_DeleteBailiffCommissionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeleteBailiffCommissionSP]
GO

CREATE PROCEDURE dbo.DN_DeleteBailiffCommissionSP
			@empeeNo	INT,
			@dateTrans	DATETIME,
			@transRefNo	INT,
			@return		INT OUTPUT
AS

	SET @return = 0
	
	-- Save a copy of the deleted transaction
	
	INSERT INTO BailiffCommnDeleted
	    (empeeno, transrefno, acctno, datetrans, transvalue, chequecolln, status)
	SELECT
	    empeeno, transrefno, acctno, datetrans, transvalue, chequecolln, status
	FROM
	    BailiffCommn
	WHERE
	    empeeno    = @empeeNo
	AND datetrans  = @dateTrans
	AND transrefno = @transRefNo
	

    -- Delete the transaction
    
    DELETE FROM BailiffCommn
    WHERE
	    empeeno    = @empeeNo
	AND datetrans  = @dateTrans
	AND transrefno = @transRefNo
	
	SET @return = @@ERROR
	return @return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO	