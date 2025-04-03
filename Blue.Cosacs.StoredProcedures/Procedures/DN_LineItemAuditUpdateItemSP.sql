
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemAuditUpdateItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemAuditUpdateItemSP]
GO





CREATE PROCEDURE 	dbo.DN_LineItemAuditUpdateItemSP
			@acctNo varchar(12),
			@agreementNo int, 
			@itemNo varchar(8),
			@location smallint,
			@source char(15),
			@return int OUTPUT

AS

	SET 		@return = 0			--initialise return code
	
    UPDATE	lineitemaudit
    SET     source = @source
    WHERE	acctno = @acctNo
    AND		agrmtno = @agreementNo
    AND		itemno = @itemNo
    AND		stocklocn = @location
    AND		contractno = ''

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

