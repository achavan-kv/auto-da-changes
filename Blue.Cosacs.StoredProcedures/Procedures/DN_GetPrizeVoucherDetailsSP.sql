SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetPrizeVoucherDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetPrizeVoucherDetailsSP]
GO


CREATE PROCEDURE 	dbo.DN_GetPrizeVoucherDetailsSP
					@acctno varchar(12),
					@custid varchar(20),
					@branchfilter varchar(5),
					@datefrom datetime,
					@dateto datetime,
					@buffno int,
					@return int OUTPUT

AS
	SET @return = 0		--initialise return code
	
	SET @datefrom = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @datefrom, 105), 105)
	SET @dateto = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @dateto, 105), 105)

	SELECT	c.custid, a.acctno, d.vouchernumber, m.dateissued, m.dateprinted, m.buffno
	FROM	prizevouchermaster m, prizevoucherdetails d, acct a, custacct c 
	WHERE	a.acctno LIKE @acctno
	AND		a.acctNo LIKE @branchfilter
	AND		a.acctno = c.acctno
	AND		c.hldorjnt = 'H'
	AND		c.custid LIKE @custid
	AND		a.acctno = m.AcctNo
	AND		CONVERT(DATETIME, CONVERT(VARCHAR(10), m.dateissued, 105), 105) BETWEEN @datefrom AND @dateto
	AND		(m.BuffNo = @buffno OR @buffno = -1)
	AND		m.voucheridentity = d.mastervoucherIdentity
	AND		d.voided = 'N'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

