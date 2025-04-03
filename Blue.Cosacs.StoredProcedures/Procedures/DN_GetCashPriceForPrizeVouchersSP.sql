SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetCashPriceForPrizeVouchersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetCashPriceForPrizeVouchersSP]
GO


CREATE PROCEDURE 	dbo.DN_GetCashPriceForPrizeVouchersSP
					@acctno char(12),
					@cashprice money out,
					@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SELECT	TOP 1
			@cashprice = SubTotal
	FROM	PrizeVoucherMaster
	WHERE	AcctNo = @acctno
	ORDER BY DateIssued DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

