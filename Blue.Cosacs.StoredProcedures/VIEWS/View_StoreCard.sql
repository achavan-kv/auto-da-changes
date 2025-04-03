

-- You may be wondering why this file has a function in it.
-- Well thats because RI needs to have views before functions and this view relies on this function so it will no upgrade.
-- Ok? Good.


IF EXISTS (SELECT * FROM SYSOBJECTS
	       WHERE xtype = 'FN'
		   AND Name = 'FN_StoreCardAcctCalc')
BEGIN
	DROP Function FN_StoreCardAcctCalc
END
GO


CREATE FUNCTION DBO.FN_StoreCardAcctCalc
  (
	@creditblocked tinyint, 
	@creditApproved bit, 
	@acctStatus varchar(5),
	@storeCardAcctStatus varchar(5),
	@acctno varchar(12),
	@cardNumber bigint
  )
RETURNS CHAR(3)
AS
BEGIN
	IF @creditblocked = 1
		RETURN 'B'

	IF @creditApproved = 0
		RETURN 'S'

	IF @acctStatus = 'S'
		RETURN 'C'

    IF @storeCardAcctStatus = 'S' OR @storeCardAcctStatus = 'C'
		RETURN @storeCardAcctStatus

      IF EXISTS (SELECT 1 
				 FROM VIEW_StoreCardStatusLatest s
			     WHERE s.StatusCode = 'A'
			     AND s.acctno = @acctno
				 AND s.CardNumber = @cardNumber)  --#12858
     RETURN 'A'
		
     IF EXISTS (SELECT 1 
				 FROM VIEW_StoreCardStatusLatest s
			     WHERE s.StatusCode = 'AA'
			     AND s.acctno = @acctno
				 AND s.CardNumber = @cardNumber)  --#12858
     RETURN 'AA'

	 --#12584
	 IF EXISTS(SELECT 1	
				FROM VIEW_StoreCardStatusLatest s
			     WHERE s.StatusCode = 'C'
			     AND s.acctno = @acctno
				 AND s.CardNumber = @cardNumber) --#12858
	 RETURN 'C'

     RETURN 'TBI'
END
GO


IF EXISTS (SELECT * FROM sysobjects
	       WHERE xtype = 'V'
	       AND name = 'View_StoreCard')
BEGIN
	DROP VIEW View_StoreCard
END
GO

CREATE VIEW View_StoreCard
AS  
SELECT cardcust.FirstName,   
	   cardcust.name AS LastName,   
	   acct.dateacctopen AS DateAcctOpen,  
	   acct.Outstbal AS Balance,  
	   cad.cusaddr1 AS Address1, 
	   cad.cusaddr2 AS Address2,
	   cad.cusaddr3 AS Address3,
	   cad.cuspocode AS PostCode,    
	   h.telno AS HomeTelNo, 
	   w.telno AS WorkTelNo, 
	   m.telno AS MobileTelNo, 
	   maincust.StoreCardLimit,  
       acct.acctno AS AccountNo,   
       S.CardNumber, 
       S.custid AS CardCustid ,   
       --agreement.[source] AS Source,    
       S.Source,								--IP - 18/04/12 - #9947
	   maincust.CreditBlocked,
       Holder = CASE WHEN s.CustID= ca.custid THEN 'Yes' ELSE 'No' END ,  
       Status = DBO.FN_StoreCardAcctCalc(maincust.CreditBlocked,maincust.StoreCardApproved, acct.currstatus, SCP.Status,acct.acctno, s.CardNumber) --#12858
FROM custacct  ca  
INNER JOIN acct ON ca.acctno = acct.acctno    
INNER JOIN StoreCard S ON acct.acctno = S.AcctNo   
INNER JOIN StoreCardPaymentDetails SCP ON S.AcctNo = SCP.acctno   
LEFT OUTER JOIN StoreCardAccountStatus_lookup SCS ON scs.[Status] = SCP.Status    
LEFT OUTER JOIN custtel H ON s.custid = H.custid AND H.datediscon IS NULL AND H.tellocn = 'H'    
LEFT OUTER JOIN custtel W ON s.custid = W.custid AND W.datediscon IS NULL AND W.tellocn = 'W'    
LEFT OUTER JOIN custtel M ON s.custid = M.custid AND M.datediscon IS NULL AND M.tellocn = 'M'    
LEFT OUTER JOIN custaddress cad ON s.custid = cad.custid AND cad.addtype = 'H' AND cad.datemoved IS NULL    
INNER JOIN agreement ON ca.acctno = agreement.acctno    
INNER JOIN customer cardcust ON s.custid = cardcust.custid    
INNER JOIN customer maincust ON CA.custid = maincust.custid    
WHERE ca.hldorjnt = 'H' 
GO  
