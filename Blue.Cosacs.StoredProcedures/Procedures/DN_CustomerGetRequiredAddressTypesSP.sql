SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetRequiredAddressTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetRequiredAddressTypesSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerGetRequiredAddressTypesSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	DISTINCT L.deliveryaddress as 'AddressType'
	FROM		custacct CA INNER JOIN lineitem L
			ON CA.acctno = L.acctno
	WHERE	CA.custid = @custid
	AND		CA.hldorjnt = 'H'
	AND		L.quantity > 0
	AND		L.deliveryaddress != ''

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

