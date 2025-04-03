
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetVouchers'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetVouchers
END
GO

CREATE PROCEDURE [dbo].[LoyaltyGetVouchers]
@custid VARCHAR(20),
@return INT output
AS
BEGIN
SELECT MemberNo,  
     Custid,  
     AcctnoGen,  
     VoucherRef,  
     VoucherValue * -1 AS VoucherValue,  
     VoucherDate 
     FROM LoyaltyVoucher  
     WHERE custid = @custid
     AND VoucherRedeemed = 0  
     AND  DATEDIFF(d,VoucherDate,GETDATE())  BETWEEN 0 AND (SELECT value FROM CountryMaintenance   
														 WHERE codename = 'LoyaltyVoucherPeriod')  
END
GO