SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerImagesUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerImagesUpdateSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerImagesUpdateSP
			@custid varchar(20),
			@picture image,
			@date datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	customerimages
	SET		picture = @picture,
			datetaken = @date
	WHERE	custid = @custid

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		customerimages
				(custid, picture, datetaken)
		VALUES	(@custid, @picture, @date)
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

