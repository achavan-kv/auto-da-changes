SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetPrizeVouchersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetPrizeVouchersSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerGetPrizeVouchersSP
					@acctno char(12),
					@dateissued datetime,
					@buffno int,
					@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SET @dateissued = CONVERT(DATETIME,CONVERT(VARCHAR(20),@dateissued,120),120)

	SELECT	VoucherNumber
	FROM	PrizeVoucherDetails d, PrizeVoucherMaster m
	WHERE	m.AcctNo = @acctno
	AND		m.DateIssued = @dateissued
	AND		m.BuffNo = @buffno
	AND		m.VoucherIdentity = d.MasterVoucherIdentity
	AND		d.voided = 'N'
	AND		m.dateprinted IS NULL

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

