IF OBJECT_ID('dbo.IsCustomerInArrearsAndHasUndeliveredAccount') IS NOT NULL
	DROP PROCEDURE dbo.IsCustomerInArrearsAndHasUndeliveredAccount
GO

CREATE PROCEDURE dbo.IsCustomerInArrearsAndHasUndeliveredAccount
	@customerId varchar(10)
AS

	SET NOCOUNT ON

	declare	@deliveryThreshold int

	SELECT @deliveryThreshold = value 
	FROM CountryMaintenance 
	WHERE CodeName = 'globdelpcent'

	SELECT 
		l.acctno, 
		isnull(sum(l.ordval), 0) AS lineitemValue, 
		CAST(0 as money) as deliveredValue
	INTO #accounts
	FROM 
		lineitem l
		INNER JOIN acct a
			on l.acctno = a.acctno
	WHERE 
		l.acctno in (SELECT acctno 
					 FROM custacct 
					 WHERE custid = @customerId and hldorjnt = 'H')
		AND l.isKit = 0
		AND l.itemno not in (select code from code where category = 'HCI' and reference = '1')
		AND a.currstatus != 'S'
	GROUP BY 
		l.acctno

	UPDATE a
	SET deliveredValue = isnull((SELECT sum(transvalue) 
						         FROM delivery 
						         WHERE acctno = a.acctno AND itemno != 'RB'), 0)
	FROM #accounts a

	SELECT 
		(
			SELECT CONVERT(Bit, COUNT(1)) AS x FROM #accounts a WHERE a.deliveredValue < a.lineitemValue * @deliveryThreshold / 100
		) AS HasUndeliveredAccount, 
		(
			SELECT CONVERT(Bit, count(1)) AS x 
			FROM custacct ca 
			WHERE 
				ca.hldorjnt = 'H'
				AND exists (SELECT 'a' 
							FROM acct a 
							WHERE 
								a.acctno = ca.acctno AND
								a.arrears > 0 AND 
								a.currstatus != 'S')
				AND ca.custid = @customerId
		) AS IsInArrears
