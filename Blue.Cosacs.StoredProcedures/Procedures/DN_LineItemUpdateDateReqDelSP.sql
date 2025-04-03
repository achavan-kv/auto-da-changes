SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateDateReqDelSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateDateReqDelSP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemUpdateDateReqDelSP
			@acctno varchar(12),
			--@itemno varchar(8),
			@itemID int,										--IP - 22/07/11 - RI
			@location smallint,
			@agreementno int,
			@contractno varchar(10),
			@datereqdel datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	lineitem
	SET		datereqdel = @datereqdel
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	--AND		itemno = @itemno
	AND		ItemID = @itemID									--IP - 22/07/11 - RI
	AND		stocklocn = @location
	AND		contractno = @contractno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

