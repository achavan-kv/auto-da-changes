SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[itemdetails_delete]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[itemdetails_delete]
GO



create procedure itemdetails_delete	@acctno	CHAR(12)	/* NOT NULL	*/	,
						@agrmtno	INTEGER	/* NOT NULL	*/	,
						@itemno	CHAR(8)	/* NOT NULL	*/	,
						@stocklocn	INTEGER	/* NOT NULL	*/	
AS
BEGIN
	DELETE
	FROM itemdetails
	WHERE acctno	= @acctno
	AND	agrmtno	= @agrmtno 
	AND	itemno	= @itemno 
	AND	stocklocn	= @stocklocn; 
	return @@error;
END;


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

