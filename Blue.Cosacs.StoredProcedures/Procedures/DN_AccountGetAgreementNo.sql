/* Issue 69238 - SC 3/9/07
 * Get agreement number so cash and go accounts with agreement numbers greater than 1
 * line items can be viewed. */

/****** Object:  StoredProcedure [dbo].[DN_AccountGetAgreementNo]    Script Date: 09/03/2007 13:53:06 ******/



SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetAgreementNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetAgreementNo]
GO

CREATE PROCEDURE 	[dbo].[DN_AccountGetAgreementNo]
			@Acctno varchar(12),
			@Agreementno INT OUT,
			@return int OUTPUT

AS
	
	SET 	@return = 0			--initialise return code

	SELECT @Agreementno = agrmtno FROM agreement
	WHERE acctno = @Acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO

SET ANSI_NULLS ON 
GO

