SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleCancelDeliverySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleCancelDeliverySP]
GO


CREATE PROCEDURE 	dbo.DN_ScheduleCancelDeliverySP
			@acctNo varchar(12),
			@agreementNo int,
			@itemId int,
			@location smallint,
			@buffBranch int,
			@buffNo int,
			@empeeno int,
			@newbuffno int,
			@type bit,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT INTO order_removed
	(
		acctno, agrmtno, itemno, ItemID, stocklocn,
		quantity, vanno, buffbranchno,buffno,
		loadno,dateprinted,empeeno,dateremoved,
		originallocation
	)
	SELECT	acctno, agrmtno, '', ItemID, stocklocn,
		quantity, vanno, buffbranchno, buffno,
		loadno,dateprinted,@empeeno,GETDATE(),
		stocklocn
	FROM	schedule
	WHERE	acctno 		= @acctNo
	AND 	agrmtno		= @agreementNo
	AND 	ItemID		= @itemId
	AND 	stocklocn 	= @location
	--AND 	buffbranchno	= @buffBranch 
	AND 	buffno		= @buffno

	IF(@type = 1)
	BEGIN
		UPDATE	Schedule
		SET		buffno = @newbuffno,
				dateprinted = null,
				printedby = 0
		WHERE	acctno 		= @acctNo
		AND 	agrmtno		= @agreementNo
		AND 	ItemID		= @itemId
		AND 	stocklocn 	= @location
		--AND 	buffbranchno	= @buffBranch 
		AND 	buffno		= @buffno
	END	
	ELSE
	BEGIN
		DELETE 
		FROM	Schedule
		WHERE	acctno 		= @acctNo
		AND 	agrmtno		= @agreementNo
		AND 	ItemID		= @itemId
		AND 	stocklocn 	= @location
		AND 	buffno		= @buffno
	END


	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
