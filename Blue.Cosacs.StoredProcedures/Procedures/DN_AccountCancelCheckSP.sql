SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountCancelCheckSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountCancelCheckSP]
GO

CREATE PROCEDURE  dbo.DN_AccountCancelCheckSP
   			@acctNo varchar(50),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code
 
	SELECT	AS400Bal AS "AS400 Balance",
			OutstBal AS "Outstanding Balance",
			CurrStatus AS "Current Status"
	FROM		ACCT
	WHERE	acctno = @acctNo
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

