if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_GetNonRFProposals]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetNonRFProposals]
GO

CREATE PROCEDURE dbo.DN_GetNonRFProposals
            @return int OUTPUT

AS
	SET @return = 0            --initialise return code

    DECLARE @limit int
    
    SELECT	@limit = value
    FROM	countrymaintenance
    WHERE	codename = 'numaccts'
    
	SET ROWCOUNT @limit
	
	IF NOT EXISTS(	SELECT * 
					FROM customer_potential_spend 
					WHERE calc_date > DATEADD(hour, -1, GETDATE()))
	BEGIN
		TRUNCATE TABLE customer_potential_spend
	    
		INSERT INTO customer_potential_spend
		SELECT	c.custid,
				'',
				0
		FROM	acct a INNER JOIN proposal p ON a.acctno = p.acctno 
			INNER JOIN customer c ON p.custid = c.custid 
			INNER JOIN custacct ca ON ca.custid = c.custid 
			INNER JOIN accttype at ON at.genaccttype = a.accttype
		WHERE	a.acctno like '___0%'
		AND	ca.hldorjnt = 'H'
		AND	at.accttype != 'R'
		AND	c.RFCreditLimit = 0
		AND	NOT EXISTS (SELECT 1
					FROM custacct
					WHERE custacct.custid = c.custid	
					AND custacct.acctno = a.acctno
					AND a.accttype = 'R')
		GROUP BY c.custid
	END	
   	
	SELECT	Max (P.acctno) as 'Account No.',
		Max (AT.accttype) as 'AccountType',	
		C.custid as 'CustomerID',
		MAX(P.dateprop) as 'DateProp'
	FROM	acct a INNER JOIN proposal p ON a.acctno = p.acctno 
		INNER JOIN customer_potential_spend cu ON p.custid = cu.custid 
		INNER JOIN customer c ON c.custid = cu.custid 
		INNER JOIN accttype at ON at.genaccttype = a.accttype
	WHERE	calc_date < DATEADD(hour, -1, GETDATE())
	AND		a.acctno like '___0%'
	GROUP BY c.custid

	SET ROWCOUNT 0

	SET @return = @@error
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

