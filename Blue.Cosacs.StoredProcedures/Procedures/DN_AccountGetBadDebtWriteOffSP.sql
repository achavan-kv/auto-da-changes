SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetBadDebtWriteOffSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetBadDebtWriteOffSP]
GO

CREATE PROCEDURE dbo.DN_AccountGetBadDebtWriteOffSP
		@branchno smallint,
                @securitised varchar(1),
		@acctno varchar(12) OUT,
		@return int OUTPUT

AS DECLARE
        @CustId varchar(20)

BEGIN
	SET 	@return = 0			--initialise return code
        SET     @CustId = 'BDWRecover%'

        IF (@securitised = 'Y') SET @CustId = 'BDWSecRecover%'

	SELECT	@acctno = CA.acctno
	FROM	custacct CA INNER JOIN acct A
	ON	CA.acctno = A.acctno
	WHERE	CA.custid LIKE @CustId
	AND	CA.acctno LIKE convert ( varchar, @branchno ) + '%'
   	AND     A.currstatus != 'S'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

