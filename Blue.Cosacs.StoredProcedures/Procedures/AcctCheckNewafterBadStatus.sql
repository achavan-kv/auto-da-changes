--min worst and max worst..
IF  EXISTS (SELECT * FROM sysobjects WHERE NAME = 'AcctCheckNewafterBadStatus')
DROP PROCEDURE AcctCheckNewafterBadStatus 
GO 
CREATE PROCEDURE AcctCheckNewafterBadStatus @acctno CHAR(12), @custid VARCHAR(20),
@worstcurrent CHAR(1),@worstsettled CHAR(1) ,@hasacceptedaccountafterbad TINYINT OUT
AS 
	DECLARE @maxWrongStatusDate DATETIME, @maxdateacctopen DATETIME  
	SELECT @maxWrongStatusDate =MAX(s.datestatchge) FROM status s, custacct ca 
	WHERE ca.acctno= s.acctno AND  
	ca.hldorjnt='H' AND ca.custid = @custid
	AND s.statuscode IN (@worstcurrent, @worstsettled)

	SET @hasacceptedaccountafterbad = 0
	
	SELECT @maxdateacctopen= ISNULL(MAX(a.dateacctopen),'1-jan-1900')
	FROM acct a ,custacct ca , proposal p
	WHERE a.acctno = ca.acctno
	AND p.acctno= ca.acctno AND ca.hldorjnt='H'
	AND p.custid = ca.custid
	AND ca.custid = @custid
	AND p.propresult='A'
	AND p.acctno != @acctno
	
	IF @maxdateacctopen > @maxWrongStatusDate
		SET @hasacceptedaccountafterbad = 1 
		
	
GO 	
	
