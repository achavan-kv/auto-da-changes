SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerIssuePrizeVouchersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerIssuePrizeVouchersSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerIssuePrizeVouchersSP
					@acctno char(12),
					@dateissued datetime,
					@buffno int,
					@subtotal money,
					@issuedby int,
					@voucherid int OUT,
					@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SET @dateissued = CONVERT(DATETIME,CONVERT(VARCHAR(20),@dateissued,120),120)

	INSERT INTO PrizeVoucherMaster (IssuedBy, DateIssued, AcctNo, BuffNo, SubTotal)
	VALUES(@issuedby, @dateissued, @acctno, @buffno, @subtotal)

	SELECT	@voucherid = VoucherIdentity
	FROM	PrizeVoucherMaster
	WHERE	AcctNo = @acctno
	AND		DateIssued = @dateissued
	AND		BuffNo = @buffno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

