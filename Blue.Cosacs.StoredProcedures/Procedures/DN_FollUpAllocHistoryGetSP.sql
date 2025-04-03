SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocHistoryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocHistoryGetSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocHistoryGetSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	f.allocno,
			convert(varchar,f.empeeno) + '  ' + c.FullName AS EmployeeName,
			isnull(convert(varchar,f.datealloc,103), ' ') as datealloc,
			isnull(convert(varchar,f.datedealloc,103), ' ') as datedealloc,
			f.allocarrears,
			f.bailfee,
			f.allocprtflag,
			convert(varchar,f.empeenoalloc) + '  ' + ca.FullName as allocempeename,
			convert(varchar,f.empeenodealloc) + '  ' + cd.FullName as deallocempeename
	FROM	follupalloc f 
	left outer join Admin.[User] cd on f.empeenodealloc = cd.id
	LEFT OUTER JOIN Admin.[User] c ON  f.empeeno = c.id
	LEFT OUTER JOIN Admin.[User] ca ON f.empeenoalloc = ca.id
	WHERE	acctno = @acctno
	
	--  AND   f.empeenodealloc = cd.empeeno				68690 jec 05/12/06


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

