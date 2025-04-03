SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryNoteGetCancelledSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryNoteGetCancelledSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryNoteGetCancelledSP
			@acctNo varchar(12),
			@agrmtno int,
			@itemId int, 
			@stocklocn smallint,
			@buffno int OUT, 
			@dateprinted datetime OUT,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	SELECT	@buffno = ISNULL(MAX(s.buffno),0),
			@dateprinted = s.dateprinted
	FROM	order_removed s
	WHERE 	s.acctno = @acctno
	AND 	s.agrmtno = @agrmtno
	AND 	s.ItemID = @itemId
	AND 	s.stocklocn = @stocklocn
	AND     ISNULL(s.dateprinted,'') > CONVERT(DATETIME,'01-Jan-1900',106)
	GROUP BY s.dateprinted

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO