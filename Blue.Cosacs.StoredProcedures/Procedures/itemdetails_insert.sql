SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[itemdetails_insert]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[itemdetails_insert]
GO



create procedure itemdetails_insert	@acctno	CHAR(12) /* NOT NULL	*/	,
						@agrmtno	INTEGER /* NOT NULL	*/	,
						@itemno	CHAR(8) /* NOT NULL	*/	, 
						@stocklocn	INTEGER /* NOT NULL	*/	, 
						@itemtext CHAR(50)
AS
DECLARE	@string_id	INTEGER	/* 4 NOT NULL	*/	,
		@branchno	INTEGER	/* 4 NOT NULL	*/	; 
BEGIN 
	/* SET	@branchno = INT4(LEFT(SHIFT(itemtext, -17),3));	*/
	/* SET	@string_id = INT4(SHIFT(itemtext, -21));	*/
	SET	@branchno	=	CONVERT(integer,SUBSTRING(@itemtext, 18,3));
	SET	@string_id	=	CONVERT(integer,SUBSTRING(@itemtext, 21,30));

	INSERT INTO itemdetails (acctno, agrmtno, itemno, stocklocn, branchno, string_id)
		VALUES	(@acctno, @agrmtno, @itemno, @stocklocn, @branchno, @string_id);

	return @@error;
END;


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

