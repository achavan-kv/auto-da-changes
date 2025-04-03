SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[itemdetails_update]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[itemdetails_update]
GO



create procedure itemdetails_update	@o_acctno	CHAR(12) /* NOT NULL	*/	,
						@o_agrmtno	INTEGER /* 4 NOT NULL	*/	, 
						@o_itemno	CHAR(8) /* NOT NULL	*/	, 
						@o_stocklocn INTEGER /* 4 NOT NULL	*/	, 
						@o_itemtext	CHAR(50) /* NOT NULL	*/	,
						@n_acctno	CHAR(12) /* NOT NULL	*/	, 
						@n_agrmtno	INTEGER /* 4 NOT NULL	*/	, 
						@n_itemno	CHAR(8) /* NOT NULL	*/	,
						@n_stocklocn INTEGER /* 4 NOT NULL	*/	, 
						@n_itemtext	CHAR(50) /* NOT NULL	*/	
AS
DECLARE	@string_id	INTEGER /* 4 NOT NULL	*/	,
		@branchno	INTEGER /* 4 NOT NULL	*/	;
BEGIN
	IF (	(@o_acctno	!=	@n_acctno) OR 
		(@o_agrmtno	!=	@n_agrmtno) OR
		(@o_itemno	!=	@n_itemno) OR
		(@o_stocklocn !=	@n_stocklocn))
	BEGIN
		/* RAISE ERROR -1 'Invalid update of ItemDetails.';	*/
		/* Pass message, add Severity=1, state=msg-number)	*/
		RAISERROR ('ERROR -1: Invalid update of ItemDetails.',1,-1);
	END;

	/* SET	@branchno = INT4(LEFT(SHIFT(itemtext, -17),3));	*/
	/* SET	@string_id = INT4(SHIFT(itemtext, -21));	*/
	SET	@branchno	=	CONVERT(integer,SUBSTRING(@n_itemtext, 18,3));
	SET	@string_id	=	CONVERT(integer,SUBSTRING(@n_itemtext, 21,30));

	UPDATE itemdetails
	SET	branchno =	@branchno, 
		string_id = @string_id 
	WHERE	acctno =	@o_acctno
	AND	agrmtno =	@o_agrmtno 
	AND	itemno =	@o_itemno 
	AND	stocklocn = @o_stocklocn;

	return @@error;
END;


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

