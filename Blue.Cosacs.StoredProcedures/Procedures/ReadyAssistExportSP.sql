IF OBJECT_ID('ReadyAssistExportSP') IS NOT NULL
	DROP PROCEDURE ReadyAssistExportSP
GO

CREATE PROCEDURE dbo.ReadyAssistExportSP
AS

	SET NOCOUNT ON

	DECLARE @accountID	VARCHAR(20),
		    @planID		VARCHAR(20)

	SELECT 
		@accountID = value
	FROM 
		CountryMaintenance
	WHERE 
		codename = 'RdyAstAcctId'

	SELECT @planID = value
	FROM CountryMaintenance
	WHERE codename = 'RdyAstPlanId'

	SELECT 
		c.custid AS 'Customer ID'
		,a.acctno AS 'Account No'
		,c.firstname + ' ' + c.NAME AS NAME
		,ltrim(rtrim(cadd.cusaddr1)) + ' ' + ltrim(rtrim(cadd.cusaddr2)) + ' ' + ltrim(rtrim(cadd.cusaddr3)) AS Address
		,'(' + ltrim(rtrim(cth.DialCode)) + ')' + ' ' + rtrim(ltrim(cth.telno)) AS 'Telephone Number 1'
		,'(' + ltrim(rtrim(COALESCE(ctw.DialCode, ctm.DialCode))) + ')' + ' ' + COALESCE(ltrim(rtrim(ctw.telno)), ltrim(rtrim(ctm.telno))) AS 'Telephone Number 2'
		,cast(ra.RAContractDate AS VARCHAR) AS 'Effective Start Date'
		,cast(cast(dateadd(day, - 1, dateadd(month, isnull(cast(r.reference AS INT), '0'), ra.RAContractDate)) AS DATE) AS VARCHAR) AS 'Effective End Date'
		,r.reference AS 'Period Of Sell'
		,@accountID AS 'Account ID'
		,@planID AS 'Plan ID'
		,CASE 
			--when (can.acctno is not null or ag.agrmttotal = 0) then 'C'
			WHEN ra.STATUS = 'Cancelled' THEN 'C'
			WHEN a.accttype = 'C' THEN CASE 
											WHEN a.arrears > 0 THEN 'O'
											ELSE 'A'
										END
			ELSE 
				CASE 
					WHEN i.instalamount > 0 AND (a.arrears / i.instalamount) >= 2 THEN 'O' 
					ELSE 'A'
				END
			END AS 'Status'
		,CASE 
			WHEN a.accttype = 'C' THEN 'D'
			ELSE 'C'
		END AS 'Type Of Sell'
	FROM 
		customer c
		INNER JOIN custacct ca
			ON c.custid = ca.custid
			AND ca.hldorjnt = 'H'
		INNER JOIN acct a
			ON ca.acctno = a.acctno
		INNER JOIN agreement ag
			ON a.acctno = ag.acctno
		INNER JOIN custaddress cadd
			ON c.custid = cadd.custid
			AND cadd.addtype = 'H'
			AND cadd.datemoved IS NULL
		INNER JOIN custtel cth
			ON c.custid = cth.custid
			AND cth.tellocn = 'H'
			AND cth.datediscon IS NULL
		LEFT JOIN custtel ctw
			ON c.custid = ctw.custid
			AND ctw.tellocn = 'W'
			AND ctw.datediscon IS NULL
		LEFT JOIN custtel ctm
			ON c.custid = ctm.custid
			AND ctm.tellocn = 'M'
			AND ctm.datediscon IS NULL
		INNER JOIN lineitem l
			ON a.acctno = l.acctno
		INNER JOIN delivery d
			ON l.acctno = d.acctno
			AND l.ItemID = d.ItemID
			AND l.stocklocn = d.stocklocn
			AND d.delorcoll = 'D'
		INNER JOIN stockinfo si
			ON si.ID = l.ItemID
		INNER JOIN code r
			ON r.code = si.IUPC
		INNER JOIN ReadyAssistDetails ra
			ON ra.AcctNo = ag.acctno
			AND ra.AgrmtNo = ag.agrmtno
			AND ra.ItemId = l.ItemID
			AND ra.Contractno = l.contractno
		LEFT JOIN instalplan i
			ON a.acctno = i.acctno
	WHERE 
		r.category = 'RDYAST'
		AND ra.STATUS IS NOT NULL 
	ORDER BY 
		c.custid
		,a.dateacctopen
GO

