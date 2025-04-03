SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ResetAgrmnTotal]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ResetAgrmnTotal]
GO

CREATE PROCEDURE DN_ResetAgrmnTotal
	    		@acctno     VARCHAR(12),
	    		@return     INTEGER OUTPUT

AS
    	SET @Return = 0;
    
	DELETE
	FROM		cancellation
	WHERE	acctno = @acctno

	UPDATE	acct
	SET 		acct.agrmttotal = agreement.agrmttotal
	FROM 		agreement
	WHERE	acct.acctno = @acctno
	AND		acct.acctno = agreement.acctno

    	SET @Return = @@ERROR
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

