SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerUpdateLoyaltyCardNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerUpdateLoyaltyCardNoSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerUpdateLoyaltyCardNoSP
			@custid varchar(20),
			@loyaltycard varchar(16),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	customer
	SET		morerewardsno = @loyaltycard
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

