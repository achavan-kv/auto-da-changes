
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetHistory'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetHistory
END
GO
  
CREATE PROCEDURE LoyaltyGetHistory  
@custid VARCHAR(20),      
@return INT output  
    
AS      

BEGIN      
  SELECT MemberNo AS LoyaltyMembershipNo, StartDate, Enddate, Mtype.codedescript Membertype, Acctstatus.codedescript AccountStatus, VoucherStatus.codedescript VoucherStatus, L.empeeno UpdatedBy, C.FullName AS UserName
  FROM loyalty L
  INNER JOIN code Mtype ON Mtype.code = L.Membertype AND category = 'HCM'
  INNER JOIN code Acctstatus ON Acctstatus.code = L.StatusAcct AND Acctstatus.category ='HCA'
  INNER JOIN code VoucherStatus ON VoucherStatus.code = L.StatusVoucher AND VoucherStatus.category ='HCR'
  LEFT OUTER JOIN Admin.[User] C ON C.id = L.empeeno 
  WHERE custid = @custid
  
  SET @return = 0
END

