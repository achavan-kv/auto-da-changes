SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetPaidAndTakenSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetPaidAndTakenSP]
GO



CREATE PROCEDURE 	dbo.DN_AccountGetPaidAndTakenSP
			@branch varchar(3),
			@acctno varchar(12) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
   declare @vbranch varchar (6)
   set @vbranch = @branch + '5%' 
	SELECT	@acctno = CA.acctno 
	FROM		custacct CA inner join acct A
	ON		CA.acctno = A.acctno
	WHERE	CA.custid = 'PAID & TAKEN'	
	AND		CA.acctno LIKE @vbranch 
	AND		A.currstatus != 'S'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

