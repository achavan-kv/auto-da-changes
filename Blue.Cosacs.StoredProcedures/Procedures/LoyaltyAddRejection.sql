
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyAddRejection'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyAddRejection
END
GO

CREATE PROCEDURE [dbo].[LoyaltyAddRejection]  
@custid VARCHAR(20) = '', 
@acctno CHAR(12), 
@return INT OUTPUT  
AS  
BEGIN  
  
IF NOT EXISTS ( SELECT * FROM custacct
				WHERE custid = @custid)
BEGIN
	SET @custid = ISNULL((SELECT custid 
	               FROM custacct
	               WHERE acctno = @acctno
	               AND hldorjnt = 'H'),'Default')
END
  
IF EXISTS (SELECT * FROM LoyaltyRejections  
   WHERE Custid = @custid)  
BEGIN  
 UPDATE LoyaltyRejections  
 SET Rejections = Rejections + 1  
 WHERE Custid = @custid  
END  
ELSE  
BEGIN  
 INSERT INTO LoyaltyRejections (  
  Custid,  
  Rejections  
 ) VALUES (   
  /* Custid - VARCHAR(20) */ @custid,  
  /* Rejections - int */ 1 )   
END  
  
END
GO


