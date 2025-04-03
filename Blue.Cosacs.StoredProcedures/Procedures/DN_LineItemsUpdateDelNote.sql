SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemsUpdateDelNote]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemsUpdateDelNote]
GO

CREATE PROCEDURE 	dbo.DN_LineItemsUpdateDelNote
			@acctno varchar(12),
			@agreementno int,
			@stocklocn smallint,
			@itemId int,
			@contractno varchar(10),
			@qty float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	lineitem
	SET	delqty = delqty + @qty,
		qtydiff = 'Y'
	WHERE	acctno = @acctno
	AND	agrmtno = @agreementno
	AND	stocklocn = @stocklocn
	AND	ItemID = @itemId
	AND	contractno = @contractno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

