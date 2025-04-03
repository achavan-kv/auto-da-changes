
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CustacctGetCustidbyType')
DROP PROCEDURE CustacctGetCustidbyType
GO 
CREATE PROCEDURE CustacctGetCustidbyType
  @acctNo  char(12),  
   @custId varchar(20) OUT,  
   @relationship VARCHAR(1),
   @return int OUTPUT  
  
AS  
  
 SET  @return = 0   --initialise return code  
  
 SELECT @custId = custid  
 FROM  custacct  
 WHERE acctno = @acctNo  
 AND  hldorjnt = @relationship
  
 IF (@@error != 0)  
 BEGIN  
  SET @return = @@error  
 END  
  
  
  
  
  