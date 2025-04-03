SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetMoreRewardsNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetMoreRewardsNoSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerGetMoreRewardsNoSP
			@custid varchar(20),
			@morerewardsno varchar(16) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@morerewardsno = morerewardsno
	FROM		customer 
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

