SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountRemoveCancellationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountRemoveCancellationSP]
GO

CREATE procedure   dbo.DN_AccountRemoveCancellationSP
   			@accountNo 	varchar(12),
   			@datereversed datetime, 
   			@user int, 
   			@code varchar(4), 
   			@notes varchar(300), 	
   			@return 	int output
 
AS
 	SET  @return = 0 --initialise return code
 
	IF EXISTS(
		SELECT	acctno 
		FROM 	cancellation 
		WHERE 	acctno = @accountNo)
	BEGIN
		DELETE
		FROM 	cancellation 
		WHERE 	acctno = @accountNo

		UPDATE	acct
		SET		currstatus = '1'	
		WHERE	acctno = @accountNo
		
		INSERT INTO	reverse_cancellation
		(
			acctno, datereversed, empeenoreverse, code, notes
		)
		VALUES
		(
			@accountNo, @datereversed, @user, @code, @notes	
		)	

	END

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

