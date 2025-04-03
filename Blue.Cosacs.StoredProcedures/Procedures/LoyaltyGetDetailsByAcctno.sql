
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetDetailsByAcctno'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetDetailsByAcctno
END
GO
  
CREATE PROCEDURE [dbo].[LoyaltyGetDetailsByAcctno]  
@acctno VARCHAR(12),      
@return INT output      
AS      
BEGIN      
  
IF EXISTS (SELECT * FROM Loyalty 
           INNER JOIN custacct ON custacct.custid = loyalty.Custid
           WHERE custacct.acctno = @acctno
           AND statusacct = 4
           AND custacct.hldorjnt = 'H')

BEGIN
SELECT MemberNo,  
  L.custid,  
  StartDate,  
  enddate,  
  MemberType,  
  StatusAcct,  
  StatusVoucher,  
  R.rejections     
 FROM custacct CA    
 LEFT OUTER JOIN Loyalty L ON CA.custid = L.custid   
							  AND L.statusacct = 4
 LEFT OUTER JOIN Loyaltyrejections R ON CA.custid=R.custid   
 WHERE hldorjnt = 'H'  
 AND CA.acctno = @acctno     
END    

ELSE

BEGIN
	SELECT MemberNo,  
  CA.custid,  
  StartDate,  
  enddate,  
  MemberType,  
  StatusAcct,  
  StatusVoucher,  
  R.rejections     
 FROM custacct CA    
 LEFT OUTER JOIN Loyalty L ON CA.custid = L.custid   
							  AND startdate = (SELECT MAX(startdate) FROM Loyalty L2
							                   WHERE l2.custid = L.custid
							                   AND l2.statusacct NOT IN (2,3))
 LEFT OUTER JOIN Loyaltyrejections R ON CA.custid=R.custid   
 WHERE hldorjnt = 'H'  
 AND CA.acctno = @acctno  
END
END