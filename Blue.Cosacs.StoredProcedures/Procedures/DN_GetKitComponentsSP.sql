SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetKitComponentsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetKitComponentsSP]
GO

CREATE PROCEDURE dbo.DN_GetKitComponentsSP
			@itemId int,
			@return int OUTPUT
AS

	SET 	@return = 0			

	SELECT	S.IUPC as componentno, componentqty, ComponentID 
	FROM	kitproduct K
	INNER JOIN StockInfo S ON K.ComponentID = S.ID
	WHERE	ItemID = @itemId  

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

