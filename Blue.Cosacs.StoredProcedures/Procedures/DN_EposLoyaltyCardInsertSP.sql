SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EposLoyaltyCardInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EposLoyaltyCardInsertSP]
GO

CREATE PROCEDURE 	dbo.DN_EposLoyaltyCardInsertSP
			@transrefno int,
			@datetrans datetime,
			@acctno varchar(12),
			@morerewardsno varchar(16),
			@agreementno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	eposloyaltycard
	SET		acctno = @acctno,
			morerewardsno = @morerewardsno,
			agrmtno = @agreementno
	WHERE	transrefno = @transrefno
	AND		datetrans = @datetrans

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		eposloyaltycard
				(transrefno, datetrans, acctno, morerewardsno, agrmtno)
		VALUES	(@transrefno, @datetrans, @acctno, @morerewardsno, @agreementno)
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

