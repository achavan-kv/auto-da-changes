SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountIsWarrantyRenewalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountIsWarrantyRenewalSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountIsWarrantyRenewalSP
			@acctno varchar(12),
			@renewal smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@renewal = count(*)
	FROM	WarrantyRenewalPurchase
	WHERE	acctno = @acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO