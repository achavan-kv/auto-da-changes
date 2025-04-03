IF EXISTS (SELECT * FROM SYS.PROCEDURES WHERE NAME = 'DN_AccountGetSP') 
	DROP PROC DN_AccountGetSP
GO


CREATE PROCEDURE 	[dbo].[DN_AccountGetSP]
			@acctno char(12),
			@accttype varchar(1) OUT,
			@paidpcent smallint OUT,
			@termstype varchar(2) OUT,
			@isAmortized int OUT,
			@IsAmortizedOutStandingBal int OUT,
			@return int OUTPUT

AS

	SET @return = 0		--initialise return code
	SET @accttype = ''
	SET @paidpcent  = 0
	SET @termstype  = ''
	SET @isAmortized = 0
	SET @IsAmortizedOutStandingBal = 0

	SELECT	@accttype = accttype, 
			@paidpcent = paidpcent,
			@termstype = termstype,
			@isAmortized = isAmortized,
			@IsAmortizedOutStandingBal=IsAmortizedOutStandingBal 
	FROM		acct
	WHERE	acctno = @acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	ELSE
	BEGIN	
		IF(@@rowcount <1)
		BEGIN
			SET @return = -1
		END
	END