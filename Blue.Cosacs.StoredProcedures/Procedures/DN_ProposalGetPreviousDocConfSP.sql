SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetPreviousDocConfSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetPreviousDocConfSP]
GO



CREATE PROCEDURE 	dbo.DN_ProposalGetPreviousDocConfSP
			@acctno varchar(12),
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	TOP 1
			ProofAddress as 'ProofOfAddress',
			ProofIncome as 'ProofOfIncome',
			DCText2,
			DCText3
	FROM		proposal
	WHERE	custid = @custid
	and 		acctno != @acctno
	ORDER BY 	dateprop DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

