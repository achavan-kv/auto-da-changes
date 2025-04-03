SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerAddressGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerAddressGetSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerAddressGetSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET @return = 0			--initialise return code

	SELECT	custid AS "Customer ID",
			addtype AS "Address Type",
			datein,
			cusaddr1 AS "Address1",
			cusaddr2 AS "Address2",
			cusaddr3 AS "Address3",
			cuspocode AS "Postcode",
			isnull(DeliveryArea,'') as DeliveryArea,
			email AS "Email",
			notes as "Notes",
			codedescript AS "Address Description"
	FROM		custaddress
			left join code on custaddress.addtype = code.code and category = 'CA1'
	WHERE	custid = @custid and datemoved is null

	SELECT	tellocn,
			DialCode AS "Dial Code",
			telno AS "Phone",
			extnno AS "Ext"
	FROM	custtel
	WHERE	custid = @custid and datediscon is null

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

