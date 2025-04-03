
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerVoidPrizeVouchersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerVoidPrizeVouchersSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerVoidPrizeVouchersSP
					@acctno char(12),
					@dateissued datetime,
					@buffno int,
					@cashprice money OUT,
					@voucherID int OUT,
					@numvouchers int OUT,
					@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SET @dateissued = CONVERT(DATETIME,CONVERT(VARCHAR(20),@dateissued,120),120)

	UPDATE	PrizeVoucherDetails
	SET		voided = 'Y'
	FROM	PrizeVoucherMaster m
	WHERE	m.AcctNo = @acctno
	AND		m.DateIssued = @dateissued
	AND		m.BuffNo = @buffno
	AND		m.VoucherIdentity = PrizeVoucherDetails.MasterVoucherIdentity
	AND		PrizeVoucherDetails.voided = 'N'

	SET	@numvouchers = @@ROWCOUNT

	UPDATE	PrizeVoucherMaster
	SET		dateprinted = null
	WHERE	AcctNo = @acctno
	AND		DateIssued = @dateissued
	AND		BuffNo = @buffno
	
	SELECT	@cashprice = SubTotal,
			@voucherID = VoucherIdentity
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

