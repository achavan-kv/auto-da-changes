SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_RestoreBailiffCommissionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_RestoreBailiffCommissionSP]
GO

CREATE PROCEDURE dbo.DN_RestoreBailiffCommissionSP
			@empeeNo	INT,
			@dateTrans	DATETIME,
			@transRefNo	INT,
			@return		INT OUTPUT
AS

	SET @return = 0
	
	-- Restore a copy of the deleted transaction
	
	INSERT INTO BailiffCommn
	    (empeeno, transrefno, acctno, datetrans, transvalue, chequecolln, status)
	SELECT
	    empeeno, transrefno, acctno, datetrans, transvalue, chequecolln, status
	FROM
	    BailiffCommnDeleted
	WHERE
	    empeeno    = @empeeNo
	AND datetrans  = @dateTrans
	AND transrefno = @transRefNo
	

    -- Remove the restored transaction
    
    DELETE FROM BailiffCommnDeleted
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