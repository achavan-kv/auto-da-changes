
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetDetailsByCustid'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetDetailsByCustid
END
GO


CREATE PROCEDURE [dbo].[LoyaltyGetDetailsByCustid]
@custid VARCHAR(20),  
@return INT output  
AS  
BEGIN  


SELECT  MemberNo,
		COALESCE(l.custid,r.custid) AS custid,
		StartDate,
		Enddate,
		MemberType,
		StatusAcct,
		StatusVoucher,
		R.rejections
		--lost,
		--renew   
 FROM Loyalty L FULL OUTER join Loyaltyrejections r ON l.custid=r.custid
 WHERE COALESCE(l.custid,r.custid) = @custid 
 AND (ISNULL(statusacct,4) = 4 
	 OR (startdate = (SELECT MAX(startdate) FROM loyalty L2
                   WHERE L2.custid = @custid )
                   AND NOT EXISTS (SELECT * FROM loyalty 
                                   WHERE custid = @custid  
                                   AND statusacct = 4)))
   
END  

GO


