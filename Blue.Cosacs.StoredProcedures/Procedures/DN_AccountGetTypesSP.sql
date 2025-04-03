SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetTypesSP]
GO






CREATE PROCEDURE 	[dbo].[DN_AccountGetTypesSP]
			@branchCode smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code


select c.accttype as acctcat,
			c.description
from acctnoctrl a, branch b,accttype c
where (a.branchno =@branchCode)
  and a.branchno = b.branchno
  and c.accttype = a.acctcat
	and c.isactive = 1
order by a.branchno,a.acctcat



	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

