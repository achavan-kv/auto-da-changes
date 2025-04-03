SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetDistinctAddressTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetDistinctAddressTypesSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerGetDistinctAddressTypesSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	DISTINCT
			addtype as 'AddressType'	
	FROM		custaddress
	WHERE	isnull(datemoved, '1/1/1900') = '1/1/1900'
	AND		custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

