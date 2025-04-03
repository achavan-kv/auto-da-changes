IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='StoreCardLoadforAnnualFee_View')
DROP VIEW StoreCardLoadforAnnualFee_View
GO
CREATE VIEW StoreCardLoadforAnnualFee_View 
AS 
SELECT ca.acctno,
DATEADD(YEAR,DATEDIFF(YEAR,ss.DateChanged,GETDATE())-1,ss.DateChanged)    AS ActivateAnniversary,-- this will always be set to last year...
ss.DateChanged AS ActivateDate,
ca.custid
 FROM custacct ca 
JOIN StoreCardPaymentDetails pd ON ca.acctno = pd.acctno
JOIN StoreCard s ON pd.acctno = s.AcctNo
JOIN StoreCardStatus ss ON s.CardNumber = ss.CardNumber 
WHERE ca.custid = s.CustID AND ss.StatusCode ='A'
AND NOT EXISTS (SELECT * FROM StoreCard P JOIN -- but don't load where previous cards were activated i.e. if replacement card
StoreCardStatus ss ON P.CardNumber = ss.CardNumber
WHERE p.AcctNo = ca.acctno AND p.CustID = ca.custid -- so for main holder 
AND p.CardNumber < s.CardNumber
AND ss.StatusCode = 'A') -- so this should be once a year at most.
GO 

