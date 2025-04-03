SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocAutoDeallocateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocAutoDeallocateSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocAutoDeallocateSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE @empeeno  int
	SET	@empeeno = 0

	UPDATE	follupalloc
	SET	follupalloc.datedealloc = getdate(),
		@empeeno = follupalloc.empeeno
	FROM	follupalloc INNER JOIN 
		acct A	ON follupalloc.acctno = A.acctno
	WHERE	isnull(follupalloc.datealloc, '1/1/1900') != '1/1/1900'
	AND	isnull(follupalloc.datealloc, '1/1/1900') <= getdate()
	AND	(isnull(datedealloc, '1/1/1900') = '1/1/1900'
	OR	isnull(datedealloc, '1/1/1900') >= getdate())
	AND	A.arrears<=0
	AND	A.currstatus = '3'
	AND	A.acctno = @acctno

	IF(@@rowcount > 0)
	BEGIN
		UPDATE	courtsperson
		SET	alloccount = alloccount - 1
		WHERE	userid = @empeeno

		-- RD 09/06/06 682727 to ensure that we do not have negative alloccount
		UPDATE 	courtsperson 
		SET 	alloccount = 0
		WHERE	userid =@empeeno
		AND	alloccount < 0
	END
		

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

