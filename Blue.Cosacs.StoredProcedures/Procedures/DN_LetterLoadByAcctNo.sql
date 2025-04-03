SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LetterLoadByAcctNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LetterLoadByAcctNo]
GO

CREATE PROCEDURE DN_LetterLoadByAcctNo @acctno varchar (12), @return integer output
AS  
	SELECT acctno, dateacctlttr, lettercode
	FROM letter
	WHERE acctno = @acctno

	IF (@@error = 0)
		SELECT statuscode, empeenostat, datestatchge
		FROM status
		WHERE acctno = @acctno

	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

