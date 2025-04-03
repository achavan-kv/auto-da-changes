SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountCancelRFSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountCancelRFSP]
GO





CREATE PROCEDURE 	dbo.DN_AccountCancelRFSP
			@acctno varchar(12),
			@custid varchar(20),	
			@user int,
			@cancelled smallint OUT,
			@return int OUTPUT

AS
 SET NOCOUNT ON
	SET 	@return = 0			--initialise return code
	SET	@cancelled = 0

	DECLARE	@currstatus char(1)
	DECLARE	@agreementtotal money
	DECLARE	@outsbal money
	
	SELECT	@currstatus  = currstatus,
			@agreementtotal = agrmttotal,
			@outsbal = outstbal
	FROM		acct
	WHERE	acctno = @acctno

	--IF(@outsbal <= 0)
	IF(abs(@outsbal) < 0.01)
	BEGIN
		UPDATE	acct
		SET		currstatus = 'S',
				lastupdatedby =@user
		WHERE	acctno = @acctno

		--put a record in the cancellation table with a code of 'B' == Unable to meet credit
		INSERT	
		INTO	cancellation
			(origbr, acctno, agrmtno, datecancel,
			empeenocanc, code, agrmttotal)
		VALUES
			(0, @acctno, 1, getdate(), @user, 'Q', @agreementtotal)

		SET	@cancelled = 1	
	END
	ELSE
	BEGIN
		RAISERROR('Outstanding balance on account %s must be refunded before account can be cancelled', 16, 1, @acctno)
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

