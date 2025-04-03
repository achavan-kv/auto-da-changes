	/****** Object:  UserDefinedFunction [dbo].[fn_CLAmortizationDailyInterest]    Script Date: 6/12/2019 8:50:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ashwini A
-- Create date: 18-06-2019
-- Description:	Calculates the daily interest if the person pays before or after the instalduedate
-- =============================================
IF OBJECT_ID('fn_CLAmortizationDailyInterest') IS NOT NULL
	DROP FUNCTION dbo.fn_CLAmortizationDailyInterest
GO
CREATE FUNCTION [dbo].[fn_CLAmortizationDailyInterest]
(
	-- Add the parameters for the function here
	@acctno			varchar(12)
)
RETURNS float
AS
BEGIN
		
	DECLARE @dailyint float ,@custid varchar(20),@termstype varchar(2) ,@scoreband varchar(4) ,@servicechgpct decimal(15,4), @noofdays int,
	@instalduedate datetime , @firstinstalduedate datetime, @lastinstalduedate datetime, @paymentdate datetime ,@balance money,@AdminFee money
	DECLARE @EOMONTH int =   day(eomonth( dateadd(month,1+datediff(month,0,getdate()),-1)))
	DECLARE @count int = 1, @dueday int , @ispaid int , @servicecharge money 

	SELECT @custid=Custid, @termstype=TermsType FROM cashloan WHERE AcctNo=@acctno
	SELECT @scoreband=ScoringBand FROM customer WHERE custid=@custid
	SELECT TOP 1 @servicechgpct=(intrate/100) FROM intratehistory inner join acct ON acct.termstype=intratehistory.termstype 
	WHERE acct.acctno=@acctno and intratehistory.termstype=@termstype and intratehistory.band=@scoreband
	SELECT @firstinstalduedate = min(instalduedate),@lastinstalduedate = max(instalduedate) FROM CLAmortizationPaymentHistory WHERE acctno = @acctno
	SELECT @dueday = dueday from instalplan where acctno=@acctno
	SELECT @instalduedate= CAST(instalduedate as date)
	FROM [dbo].[CLAmortizationSchedule] WHERE acctno = @acctno AND (DATEPART(year, instalduedate)) = DATEPART(year,getdate())
	AND    DATEPART(month, instalduedate) = DATEPART(month,getdate()) 

	IF(@dueday <> 0)   --dueday of the cashloan cannot be zero
	BEGIN
	SELECT @balance = SUM(Principal) from clamortizationpaymenthistory where acctno=@AcctNo
		--if the current date of payment is in bracket to the instalduedate
			IF(getdate() between @firstinstalduedate and @lastinstalduedate)
				BEGIN
				  SELECT TOP 1 @paymentdate = instalduedate FROM CLAmortizationPaymentHistory WHERE acctno =@acctno and ispaid = 0 			
				
					IF (getdate() < @paymentdate and DATEDIFF(day, @paymentdate,getdate()) > DATEDIFF(day,@paymentdate,DATEADD(month, -1, @paymentdate)) )
					  BEGIN 
					  SET @noofdays =  DATEDIFF(day, @paymentdate,getdate());	
					  END

					ELSE IF (getdate() > @paymentdate)
					BEGIN
						IF(@instalduedate < getdate())
							BEGIN
							SET @noofdays =  DATEDIFF(day, @instalduedate,getdate());	
							END
						ELSE
							BEGIN
								SET @noofdays =  DATEDIFF(day, DATEADD(month, -1, @instalduedate),getdate());    
							END
					END
			END
				
		--if the date is after the last instalment date
			ELSE IF(getdate() > @lastinstalduedate)
			BEGIN
				SELECT @servicechgpct = 0
			END
		--if the date is before the first instalment date	
			ELSE IF(getdate() < @firstinstalduedate and ( convert(date,getdate()) > convert(date,(select datetrans from delivery where acctno = @acctno and itemno = 'LOAN'))))
			BEGIN
				SELECT TOP 1 @ispaid = ispaid,@instalduedate=instalduedate from CLAmortizationPaymentHistory where acctno = @acctno
				--if the first instalment is not at all paid then take the opening balance or take as 0.00
				IF (@ispaid = 0)
					BEGIN
					SELECT @noofdays =  DATEDIFF(day, @instalduedate,getdate());		
					END	
				ELSE 
					SELECT @servicechgpct = 0								
			END
		--SELECT @dailyint = ((select @servicecharge)/@EOMONTH) * isnull(@noofdays,0) 
		SELECT @dailyint = ((@balance * @servicechgpct) * isnull(@noofdays,0) )/(DATEDIFF(day,cast( year(getdate()) as char(4)) ,cast(year(getdate())+1 as char(4)) ))	
	END
	ELSE
	SELECT @dailyint = 0
	
	RETURN ROUND(isnull(@dailyint,0),2)

END


GO






	