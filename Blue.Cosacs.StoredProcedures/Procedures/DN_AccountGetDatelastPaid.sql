-- Procedure datelastpaid get last non-reversed payment on account
IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'DN_AccountGetDatelastPaid')
DROP PROCEDURE DN_AccountGetDatelastPaid
GO
CREATE PROCEDURE [dbo].DN_AccountGetDatelastPaid
    @return int OUTPUT  ,
    @acctno CHAR(12), @datelastpaid DATETIME OUT, @correctvalue MONEY
AS  
   SET @return = 0
   DECLARE @dateacctopen DATETIME,@paymenttotal MONEY,@counter INT 
   SET @counter = 1
   SELECT @datelastpaid= ISNULL(MAX(datetrans),'1-jan-1900')
   FROM fintrans 
   WHERE ACCTNO = @acctno
   AND transtypecode IN ('PAY','XFR','SCX','DDN') AND transvalue <0 

   SELECT @dateacctopen=dateacctopen FROM ACCT WHERE acctno =@acctno   

   WHILE @datelastpaid > @dateacctopen
   BEGIN
      select @paymenttotal=ISNULL(sum(transvalue),0) FROM fintrans WHERE acctno =@acctno
      AND transtypecode IN ('pay','xfr','scx','ref','RET','COR','DDN')
      AND datetrans >= @datelastpaid
	  SET @paymenttotal = @paymenttotal + @correctvalue
      IF	@paymenttotal >=0 -- correction exceeded or equal to payment amount so find last non-reversed payment
      BEGIN
	     	SELECT @datelastpaid= ISNULL(MAX(datetrans),'1-jan-1900')
         FROM fintrans 
         WHERE ACCTNO = @acctno
         AND transtypecode IN ('PAY','XFR','SCX','DDN') AND transvalue <0 
         AND datetrans < @datelastpaid
	   END 
      ELSE -- payment <0 so good 
         BREAK
        
      SET @counter = @counter +1
      IF @counter >50 -- give up this should never happen anyway. 
         BREAK
   END
   
   
   
   SET @return = @@ERROR


