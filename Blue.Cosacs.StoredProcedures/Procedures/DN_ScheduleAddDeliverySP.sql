SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleAddDeliverySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleAddDeliverySP]
GO


CREATE PROCEDURE [dbo].[DN_ScheduleAddDeliverySP] 
	@acctno CHAR(12), @agrmtno INTEGER, @itemno VARCHAR(8), @stocklocn SMALLINT, @buffno INT,
	@buffbranchno SMALLINT, @loadno INTEGER, @user INTEGER, @return INTEGER OUTPUT
AS

	DECLARE	@diff int
	SELECT	@diff = 0

	UPDATE 	lineitem
	SET 		delqty = delqty + (quantity - delqty),
			@diff = (quantity - delqty),
		    	qtydiff = 'N'
	WHERE 	acctno = @acctno
	AND 		agrmtno = @agrmtno
	AND 		itemno = @itemno
	AND 		stocklocn = @stocklocn

	SELECT @return = @@error

	IF(@return = 0)
	BEGIN
		INSERT 
		INTO 		schedule 
				(origbr, acctno, agrmtno, datedelplan, delorcoll, itemno, stocklocn,
				quantity, retstocklocn, retitemno, retval, vanno, buffbranchno, buffno, loadno, printedby)
		SELECT 	l.origbr, l.acctno, l.agrmtno, l.dateplandel, 'D' AS delorcoll, l.itemno, l.stocklocn,
		      	 	@diff AS quantity, NULL, NULL, NULL, NULL, @buffbranchno, @buffno, @loadno, @user
		FROM		lineitem l
		WHERE 	acctno = @acctno
		AND 		agrmtno = @agrmtno
		AND 		itemno = @itemno
		AND 		stocklocn = @stocklocn
		AND NOT EXISTS (SELECT * FROM schedule WHERE acctno = @acctno AND agrmtno= @agrmtno AND itemno = @itemno AND stocklocn = @stocklocn) --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
		-- prevent duplicate key on insert as may already be a schedule record inserted for 3pl 
		
		SELECT @return = @@error
	END
/*
	IF (@return = 0)
	BEGIN
		UPDATE lineitem
		SET delqty = s.quantity,
		    qtydiff = 'N'
		FROM schedule s
		WHERE s.acctno = @acctno
		AND s.agrmtno = @agrmtno
		AND s.itemno = @itemno
		AND s.stocklocn = @stocklocn
		--AND s.buffbranchno = @buffbranchno
		AND s.buffno = @buffno

		SELECT @return = @@error
	END
*/

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

