IF OBJECT_ID('UndeliveredCashCreditPerBranch') IS NOT NULL
	DROP PROCEDURE UndeliveredCashCreditPerBranch
GO

CREATE PROCEDURE UndeliveredCashCreditPerBranch
	@BranchNo SmallInt
AS 

	SET NOCOUNT ON 

	DECLARE	@deliveryThreshold int

	SELECT @deliveryThreshold = value 
	FROM CountryMaintenance 
	WHERE CodeName = 'globdelpcent'

	SELECT 
		data.acctno AS CustomerAccount,
		cust.firstname  as CustomerFirstName,
		cust.name AS CustomerLastName,
		s.itemno AS ItemNo,
		LTRIM(RTRIM(s.itemdescr1)) AS ItemDescription,
		code.codedescript AS StatusDescription,
		NULLIF(LTRIM(RTRIM(CONVERT(VarChar, m.DialCode))) + LTRIM(RTRIM(m.telno)), '') AS MobileNumber,
		NULLIF(ISNULL(LTRIM(RTRIM(CONVERT(VarChar, cu.DialCode))), '') + LTRIM(RTRIM(cu.telno)), '') AS LandLinePhone,
		data.DeliveryDate,
		cust.custid AS CustomerId
	FROM 
		(
			SELECT 
				l.acctno, 
				l.ItemID, 
				ISNULL(SUM(l.ordval), 0) AS lineitemValue, 
				ISNULL(SUM(d.transvalue), 0) AS deliveredValue,
				MAX(l.datereqdel) AS DeliveryDate
			FROM 
				lineitem l
				LEFT OUTER JOIN delivery d
					ON l.acctno = d.acctno
					AND l.agrmtno = d.agrmtno
					AND l.ItemID = d.ItemID
					AND l.stocklocn = d.stocklocn
					AND l.ParentItemID = d.ParentItemID
					AND l.contractno = d.contractno
			WHERE 
				l.acctno IN 
				(
					SELECT a.acctno
					FROM acct a
					LEFT OUTER JOIN 
					(
						SELECT d.acctno, SUM(d.transvalue) AS transvalue
				   		FROM delivery d
				   		WHERE 
							d.acctno IN 
							(
								SELECT acctno 
								FROM acct 
								WHERE currstatus != 'S' AND accttype != 'S' AND LEFT(d.acctno, 3) = @BranchNo
							) 
							AND d.itemno != 'RB'
				   		GROUP BY d.acctno
				   	) AS delValue
						ON a.acctno = delValue.acctno
					WHERE 
						a.accttype != 'S'
				   		AND a.currstatus != 'S'
				   		AND ISNULL(delValue.transvalue, 0) < a.agrmttotal * @deliveryThreshold / 100
				   		AND LEFT(a.acctno, 3) = CONVERT(VarChar, @BranchNo)
				)
				AND l.itemno NOT IN ('RB', 'DT')
				AND l.isKit = 0
				and l.itemno not in (select code from code where category = 'HCI' and reference = '1')
				AND d.acctno IS NULL
                AND l.quantity > 0
			GROUP BY  l.acctno, l.stocklocn, l.ItemID, l.ParentItemID, l.contractno
		) data
		INNER JOIN StockInfo s
			ON s.Id = data.ItemID
				AND s.itemtype = 'S'
		INNER JOIN custacct c
			ON data.acctno = c.acctno
			AND c.hldorjnt = 'H'
		INNER JOIN customer cust
			ON c.custid = cust.custid	
		LEFT JOIN custtel cu 
			ON c.custid = cu.custid
			AND (cu.datediscon is null) 
			AND cu.tellocn ='H'
		LEFT JOIN custtel m
			ON  m.custid = c.custid and m.tellocn ='M'  
			AND (m.datediscon is null)
		LEFT JOIN 
			(
				SELECT s.acctno, MAX(s.status) AS Status FROM Summary1AcctStatus s GROUP BY s.acctno
			) summary
			ON c.acctno = summary.acctno
		LEFT JOIN code 
			ON summary.status = code.code
			AND code.category = 'APS'
			AND code.code NOT IN('WRD','DAD','PLP','TPP','DEL','COL','ROP','NOL','CRF','MDE','CAN')