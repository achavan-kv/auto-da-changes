
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetDetailsByMemberno'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetDetailsByMemberno
END
GO

CREATE PROCEDURE [dbo].[LoyaltyGetDetailsByMemberno]
@memberno VARCHAR(20),  
@return INT output  
AS  
BEGIN  


SELECT  MemberNo,
		custid,
		StartDate,
		Enddate,
		MemberType,
		StatusAcct,
		StatusVoucher 
 FROM Loyalty L 
 WHERE MemberNo = @memberno 
   
END  

GO


