 IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'StoreCardSave')
DROP PROCEDURE StoreCardSave
GO 
CREATE PROCEDURE StoreCardSave 
@CardNumber bigint,	   @CardName VARCHAR(26),
	   @IssueYear SMALLINT,	   @IssueMonth TINYINT,
	   @ExpirationYear SMALLINT,	   @ExpirationMonth TINYINT,
	   @InterestRate interest_rate ,	   @RateId int,
	   @AcctNo CHAR(12),@custid VARCHAR(20),
       @MonthlyAmount DECIMAL, @paymentMethod VARCHAR(4),@paymentOption VARCHAR(4),
       @ratefixed bit, @StatementFrequency varchar(24), @Nostatements BIT,@DateLastStatementPrinted DATETIME,
	   @DatePaymentDue DATETIME, @ContactMethod VARCHAR(6) ,@DateNotePrinted DATETIME 
AS 
DECLARE @rowcount INT  



IF @rowcount = 0
BEGIN  
INSERT INTO StoreCard
( 
	   CardNumber,	   CardName,
	   IssueYear,	   IssueMonth,
	   ExpirationYear,	   ExpirationMonth,
	   AcctNo
	   ) 
VALUES 	   
( 	  
	   @CardNumber,	   @CardName,
	   @IssueYear,	   @IssueMonth,
	   @ExpirationYear,	   @ExpirationMonth,
	   @AcctNo
	   ) 

END

DECLARE @status VARCHAR(6)
SET @status = ''

-- Commenting out until we come up with an awesome plan.

--IF EXISTS (SELECT * FROM StoreCard WHERE AcctNo = @acctno 
--AND CardIssued='Y' AND ISNULL(LostorStolenOn,'1-jan-1900') ='1-jan-1900')
--	SET @status = 'AA' -- Card issued

--If @status = 'AA'
-- IF EXISTS (SELECT * FROM StoreCard 
--			WHERE AcctNo = @acctno 
--			AND CardIssued='Y' AND ISNULL(LostorStolenOn,'1-jan-1900') ='1-jan-1900'
--	        AND EXISTS (SELECT * 
--	                    FROM StoreCardActivation sca
--	                    WHERE storecard.CardNumber = sca.CardNo))
--		set @status = 'A'

--IF @status !='AA' and @status !='A'
--BEGIN
--	IF EXISTS (SELECT * FROM StoreCard WHERE AcctNo = @acctno 
--	AND CardIssued='Y' AND ISNULL(LostorStolenOn,'1-jan-1900') >'1-jan-1900')
--		SET @status = 'LS'
--END
--IF @status = 'LS' -- if lost and stolen and new card being issued then tobe reissued...
--	IF EXISTS (SELECT * FROM StoreCard WHERE AcctNo = @acctno 
--	AND CardIssued !='Y' AND ISNULL(LostorStolenOn,'1-jan-1900') ='1-jan-1900')
--		SET @status = 'TBR'

--IF @status = ''
--	SET @status = 'TBI' -- to be issued as we are saving
	
	/*If the card is open then it is to be issued. If the Card is issued (exported) but not activated then it is awaiting 
	application. If the Card is activated then the status is Active. If the user has no active card but 
	a lost and stolen one then the status is Lost and stolen. If the customer has a lost and stolen card
	and another card which is still to be issued then the status is to be reissued. If the card is not exported
	i.e. just created then the status is to be issued.  */

UPDATE StoreCardPaymentDetails 
SET MonthlyAmount = @MonthlyAmount,
[status] = @status ,
PaymentMethod = @paymentMethod,
PaymentOption = @paymentOption,
InterestRate = @InterestRate,
RateId = @RateId,
RateFixed =@Ratefixed,
StatementFrequency = @StatementFrequency,
NoStatements = @NoStatements,
DateLastStatementPrinted = @DateLastStatementPrinted ,
DatePaymentDue=   @DatePaymentDue ,
ContactMethod= @ContactMethod ,
DateNotePrinted =@DateNotePrinted  
WHERE acctno= @acctno  	  -- don't put anything' 
SET @rowcount = @@ROWCOUNT -- between these lines

IF @rowcount = 0
BEGIN
INSERT INTO dbo.StoreCardPaymentDetails (
	acctno,	MonthlyAmount,
	PaymentMethod,	PaymentOption,
	RateId,	InterestRate,
	RateFixed,StatementFrequency, [Status],
	DateLastStatementPrinted , DatePaymentDue , ContactMethod ,
	DateNotePrinted, NoStatements
	) 
VALUES ( @acctno ,@MonthlyAmount,
	@PaymentMethod,	@PaymentOption,
	@RateId,	@InterestRate,
	@RateFixed, @StatementFrequency, @status, 
	@DateLastStatementPrinted , @DatePaymentDue , @ContactMethod ,
	@DateNotePrinted,@Nostatements)   
END
GO 

