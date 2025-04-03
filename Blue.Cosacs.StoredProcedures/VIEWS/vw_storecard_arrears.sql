/****** Object:  View [dbo].[vw_storecard_arrears]    Script Date: 01/30/2012 12:03:31 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_storecard_arrears]'))
DROP VIEW [dbo].[vw_storecard_arrears]
GO

/****** Object:  View [dbo].[vw_storecard_arrears]    Script Date: 01/30/2012 12:03:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Accttypegroup
create view [dbo].[vw_storecard_arrears]
-- **********************************************************************
-- Title:
-- Developer: ?
-- Date: ?

-- Change Control
-- --------------
-- Date      By    Description
-- ----      --    -----------
-- 13/03/12  JC/IP  #9791 - Cater for where the first statement. Payment for first statement was
--					not being picked up.
-- 20/03/12  IP     #9805 - Previously join on [Last] was required inorder to select prevstmtminpayment as 
--					this was considered to be the previous min + any paymentts made for that statement. This is 
--					not the case. Therefore removed join onto last as not required and now calculating the outstminpay
-- **********************************************************************	
AS

WITH thismonth (acctno, datefrom)
		AS (
				SELECT acctno, MAX(datefrom)
				FROM StoreCardStatement
				GROUP BY acctno
			)
		SELECT this.acctno, this.[dateto], DATEADD(DAY,CONVERT(INT, value), this.[dateto]) AS datedue, this.StmtMinPayment AS currminpay, 
			 this.prevstmtminpayment AS prevminpay,
			 this.prevstmtminpayment + SUM(ISNULL(f2.transvalue,0)) AS outstminpay, SUM(ISNULL(f.transvalue, 0)) AS payments
		FROM StoreCardStatement this
		INNER JOIN countrymaintenance 
			ON codename = 'SCardInterestFreeDays'
		INNER JOIN thismonth m 
			ON m.acctno = this.acctno
			AND m.datefrom = this.datefrom
		--LEFT OUTER JOIN storecardstatement [last]
		--	ON [LAST].acctno = this.acctno
		--	and [LAST].datefrom != this.datefrom
		LEFT OUTER JOIN fintrans f
			ON datetrans > this.[DateTo]
			AND f.acctno = this.Acctno
			AND f.transtypecode IN ('pay', 'ref', 'cor', 'str')
		LEFT OUTER JOIN fintrans f2
			ON  f2.acctno = this.acctno
			AND f2.datetrans between this.DateFrom and this.DateTo
			AND f2.transtypecode IN ('pay', 'ref', 'cor', 'str')
		--WHERE ISNULL([LAST].datefrom, GETDATE()) = ISNULL((SELECT MAX(datefrom) 
		--											FROM StoreCardStatement d 
		--											WHERE d.acctno = [last].acctno 
		--											AND d.datefrom < m.datefrom), GETDATE())
		GROUP BY  this.acctno, this.[dateto], this.stmtminpayment, this.prevstmtminpayment, value
		
	

GO


