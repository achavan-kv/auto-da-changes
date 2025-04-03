SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineitemAccountSettledByAddToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineitemAccountSettledByAddToSP]
GO

CREATE PROCEDURE 	dbo.DN_LineitemAccountSettledByAddToSP
			@acctno varchar(12),
			@reversible smallint OUT,
			@return int OUTPUT

AS DECLARE
	@addedTo CHAR(12),
	@currStatus CHAR(1),
	@outStBal MONEY,
	@reverseAmount MONEY

	SET 	@return = 0			--initialise return code

	SET	@reversible = 0

	SELECT 	@reversible = count(*)
	FROM	lineitem 
	WHERE	acctno = @acctno
	AND		itemno = 'ADDCR'
	
	IF (@reversible > 0)
	BEGIN
		-- Add-To is only reversible if the new account is not settled
		-- and the outstanding balance >= the amount to reverse.
		
		SELECT 	@reverseAmount = SUM(OrdVal)
		FROM	lineitem 
		WHERE	acctno = @acctno
		AND		itemno = 'ADDCR'

		SELECT	TOP 1 @addedTo = chequeno
		FROM	fintrans
		WHERE	acctno = @acctno
		AND		transtypecode = 'ADD'
		AND		transvalue < 0
		ORDER BY DateTrans DESC
		
		SELECT @outStBal = OutStBal,
		       @currStatus = CurrStatus
		FROM  Acct
		WHERE acctNo = @addedTo
		
		IF (@outStBal < @reverseAmount OR @currStatus = 'S')
		BEGIN
		    SET @reversible = 0
		END
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

