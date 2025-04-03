SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetCurrentQtySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetCurrentQtySP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemGetCurrentQtySP
			@acctno varchar(12),
			@itemId int,
			@stocklocn smallint,
			@contractno varchar(10),
			@agreementno int,
			@ParentItemID int,
			@quantity float OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@quantity = quantity 
	FROM	lineitem
	WHERE	acctno = @acctno
	AND		itemId = @itemId
	AND		stocklocn = @stocklocn
	AND		contractno = @contractno
	AND		agrmtno = @agreementno
	and		ParentItemID = @ParentItemID
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

