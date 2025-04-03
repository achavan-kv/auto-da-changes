SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetLinkedCustomerSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetLinkedCustomerSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountGetLinkedCustomerSP
			@acctNo varchar(12),
			@custId varchar(20) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@custId = custid
	FROM		custacct
	WHERE	acctno = @acctNo
	AND		hldorjnt = 'H'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

