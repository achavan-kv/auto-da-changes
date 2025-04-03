
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeGetWarrantySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeGetWarrantySP]
GO

CREATE PROCEDURE 	dbo.DN_ExchangeGetWarrantySP
			@acctno varchar(12),
			@agrmtno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
   	
	SELECT DISTINCT
	        warrantyno, 
			warrantylocn,
			contractno,
			WarrantyID
	FROM 	exchange
	WHERE	acctno = @acctno
	AND		agrmtno = @agrmtno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

