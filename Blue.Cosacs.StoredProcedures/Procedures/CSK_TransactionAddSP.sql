
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CSKTransactionAddSP') 
DROP PROCEDURE CSKTransactionAddSP
go
Create PROCEDURE CSKTransactionAddSP
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : CSKTransactionAddSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure that processes payments taken from the Kiosk
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
   29/09/10  IP  CR1202 - Generate collection FEES for payments

************************************************************************************************************/
@Transno int,
@transdate datetime,
@machid int,
@locid smallint,
@acctno char(12),
@amtPaid money,
@ChqNo varchar(16),
@BankCode varchar(6),
@CardNo varchar(16),
@AppCode Varchar(20),
@Paymethod smallint,
@return int output,
@errmsg varchar(2000) output

AS

SET NOCOUNT ON -- marginally improves performance
-- audit all transactions

begin transaction
	set @return=0
	
	insert into CSKTransactions
	(Transno,transdate,machid,locid,acctno,
	amtPaid ,ChqNo,BankCode,CardNo ,AppCode ,
	Paymethod ) 
	select @Transno,@transdate,@machid,@locid,@acctno,
	@amtPaid,@ChqNo,substring(@BankCode,1,6),@CardNo ,@AppCode,
	@Paymethod
commit;


begin transaction

BEGIN TRY
	declare @valid int, @del money,@agreement money, @settlement money, @rebate money, @paid money, @finalpayment char(1),@chqdays int,@currstatus char(1), @arrears MONEY, @collectionFEE MONEY, @outstbal money --IP - 29/09/10 - CR1202 - Added @arrears,@collectionFEE, @outstbal
	set @valid=1
	select @chqdays=value from CountryMaintenance where Name='Cheque Clearance Days'
	select @currstatus=currstatus, @agreement=agrmttotal from acct where acctno=@acctno
	select @del=isnull(sum(transvalue),0) from fintrans where acctno=@acctno and transtypecode in ('DEL','GRT') --IP - 15/04/10 - UAT(23) UAT5.2

IF NOT(EXISTS(SELECT 1 FROM acct WHERE acctno = @acctno)
	AND EXISTS (SELECT 1 from branch where branchno=@locid)
	AND EXISTS (SELECT 1 from Admin.[User] where Login =@machid))
BEGIN
	set @valid=0
	RAISERROR('Invalid Details', 16, 1)
	

END
IF NOT EXISTS(SELECT 1 FROM accountlocking WHERE acctno = @acctno and lockedby=@machid)
BEGIN
	set @valid=0
	RAISERROR('Lock Expired', 16, 1)
	

END
IF @Paymethod=2 and (@ChqNo='' or @BankCode='' or @AppCode='')
BEGIN
	set @valid=0
	RAISERROR('No Cheque Details', 16, 1)
END

IF @Paymethod in (3,4,6) and (@CardNo='')
BEGIN
	set @valid=0
	RAISERROR('No Card Details', 16, 1)
END

IF @Paymethod not in (1,2,3,4,6) 
BEGIN
	set @valid=0
	RAISERROR('Invalid Paymethod', 16, 1)
END

IF @amtPaid<=0
BEGIN
	set @valid=0
	RAISERROR('Invalid Amount', 16, 1)
	

END

DECLARE @mustdeposit BIT
EXEC DN_CashierMustDepositSP @empeeno=@machid, @mustdeposit=@mustdeposit OUT, @return=@return OUT

IF (@mustdeposit = 1)
BEGIN
set @valid=0
	RAISERROR('UserMustDeposit', 16, 1)
END


IF EXISTS ( SELECT * FROM loyalty
			WHERE loyalty.StatusAcct = 4
			AND custid = (SELECT custid 
						  FROM custacct 
						  WHERE acctno = @acctno
						  AND hldorjnt = 'H'			--IP - 29/09/10 - Subquery returned more than one value
			AND loyalty.loyaltyacct != @acctno))
BEGIN
	set @valid=0
	RAISERROR('LoyaltyNotPaid', 16, 1)
END
    
IF  (( SELECT SUM(transvalue) 
		    FROM fintrans
		    WHERE acctno IN (
								SELECT loyaltyacct FROM loyalty
								WHERE loyalty.StatusAcct = 4
								AND custid = (SELECT custid 
											  FROM custacct 
											  WHERE acctno = @acctno
											  AND hldorjnt = 'H'))) != @amtPaid)
	
BEGIN
	set @valid=0
	RAISERROR('LoyatlyPaymentWrong', 16, 1)
END


if @@ERROR=0
BEGIN
select @settlement=isnull(earlysettle,0), @rebate=isnull(rebate,0),@paid=ISNULL(-AmountPaid,0), @finalpayment=ISNULL(FinalPayment,'Y')
from csk_get_audit
where csk_get_audit.acctno=@acctno


IF @finalpayment='Y'
BEGIN
	set @valid=0
	RAISERROR('Final Payment', 16, 1)
	

END


IF @currstatus in ('6','9')
BEGIN
	set @valid=0
	RAISERROR('In Collections', 16, 1)
	

END

IF (@amtPaid-@settlement>0 or @settlement<=0) AND @del =@agreement
BEGIN
	set @valid=0
	RAISERROR('OverPayment', 16, 1)
END
END
IF @@ERROR=0
BEGIN
 -- acct is valid
	
	set @amtpaid=@amtpaid*-1
	set @rebate=@rebate*-1
	set @chqno=case when @ChqNo='' then @CardNo else @ChqNo end
	
	exec DbArrearsCalc   @acctno=@acctno,@countpcent=0,@nodates=0,@arrears=@arrears output,@return=@return OUTPUT --IP - 29/09/10 - CR1202 - Get arrears value
	
	--IP - 29/09/10 - CR1202 - Credit FEES - Check if payment will generate a FEE
	EXEC CSK_CalculateCollectionFEESP @acctno, @arrears, @amtpaid, @collectionFEE OUTPUT, @return	--IP - 28/09/10 - CR1202
	
	exec DN_FinTransWriteSP @origbr=@locid,@branchno=@locid, @acctno=@acctno,
							@transrefno=@transno,@datetrans=@transdate,@transtypecode='PAY',
							@empeeno=@machid,@transupdated='Y',@transprinted='N',
							@transvalue=@AmtPaid,@bankcode=@BankCode,@bankacctno=@AppCode,
							@chequeno=@chqno,@ftnotes= 'CSK',
							@paymethod=@paymethod,@runno=0,@source='COSACS',@agrmtno = 1,@return=@return --IP - 29/09/10 - Agrmtno added
							
	IF(@collectionFEE > 0) --IP - 29/09/10 - CR1202 - If a colleciton FEE was calculated then post the FEE amount to fintrans
	BEGIN
		DECLARE @hirefnoFee INT 
		
		UPDATE branch -- update branch first to get a lock. 
		SET hirefno = hirefno+1
		WHERE  branchno = @locid
		
		SELECT @hirefnoFee = hirefno
		FROM branch WHERE branchno = @locid 
		
		exec DN_FinTransWriteSP @origbr=@locid,@branchno=@locid, @acctno=@acctno,
							@transrefno=@hirefnoFee,@datetrans=@transdate,@transtypecode='FEE',
							@empeeno=@machid,@transupdated='Y',@transprinted='N',
							@transvalue=@collectionFEE,@bankcode='',@bankacctno='',
							@chequeno='',@ftnotes= 'CSK',
							@paymethod=0,@runno=0,@source='COSACS',@agrmtno = 1,@return=@return
		
	END
	
	declare @deposit money
if @Paymethod = 2
BEGIN
    IF @acctno LIKE '___0%' -- credit account check whether this payment exceeds deposit
    BEGIN
		
		select @deposit = deposit FROM agreement WHERE acctno= @acctno 
	    IF @deposit =0 
	    BEGIN
			DECLARE @termstype VARCHAR(3)
			SELECT @termstype = termstype FROM acct WHERE acctno= @acctno 
			IF EXISTS (SELECT * FROM termstype WHERE instalpredel = 'Y' 
					AND termstype = @termstype )
					SELECT  @deposit = instalamount FROM instalplan 
					WHERE acctno= @acctno 
		END
			
	END
	
	IF @acctno LIKE '___4%'
		SELECT  @deposit = agrmttotal FROM agreement WHERE acctno= @acctno 
    -- setting date deposit cheq cleared but only if this payment is the 
    -- one which pushes past the deposit amount or if cash account the agrmttotal
	update agreement 
	set datedepchqclr=dateadd(day,@chqdays,@transdate) 
	where @Paymethod=2 and agreement.acctno=@acctno
	and -(@paid + @AmtPaid) >= @deposit  
	AND -@paid < @deposit 
	
	insert into cheqdetail (Origbr,bankcode,bankacctno,chequeno,cheqval)
	values (@locid,@bankcode,@appcode,@chqno,@amtpaid)
	

	insert into cheqfintranslnk(bankcode,bankacctno,chequeno,acctno,transrefno)
	values (@BankCode,@AppCode,@chqno,@acctno,@Transno)
	
end
--select @del=isnull(sum(transvalue),0) from fintrans where acctno=@acctno and transtypecode in ('DEL','GRT') --IP - 15/04/10 - UAT(23) UAT5.2 - Moved to above.

if ((@amtPaid*-1)=@settlement and @del=@agreement AND @del >0 ) -- settle the account
  BEGIN 
	DECLARE @hirefno INT , @hibuffno INT 
	 IF @rebate <0
	 BEGIN -- post a rebate 
  		UPDATE branch -- update branch first to get a lock. 
		SET    hibuffno = hibuffno+1,
			   hirefno = hirefno+1
		WHERE  branchno = @locid
	
		SELECT @hirefno = hirefno,
		@hibuffno= hibuffno 
		FROM branch WHERE branchno = @locid 

	  	INSERT INTO delivery
			(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn,
			quantity, retitemno, retstocklocn, retval, buffno, buffbranchno,
			datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker, notifiedby, ftnotes)
		SELECT	@locid, @acctno, 1, getdate(), 'D', 'RB', @locid, --'rb' is the item number for rebate
			1, '', '', '', @hibuffno,@locid,
			getdate(), @locid, @hirefno,@rebate, 0, '', '', @machid, 'CSKR'
		FROM	branch
		WHERE	branchno = @locid

	  END
						
		exec DbArrearsCalc   @acctno=@acctno,@countpcent=0,@nodates=0,@arrears=0,@return=@return
		
		update acct set lastupdatedby=@machid,currstatus='S', outstbal=0 ,paidpcent=100, datelastpaid=@transdate,
		arrears = 0 where acctno=@acctno

		--insert into status(acctno,datestatchge,empeenostat,origbr,statuscode)
		--select @acctno,GETDATE(),@machid,@locid,'S'
						
	END
	
	ELSE -- not being settled
	
	begin
	
	update acct
	set outstbal=(select sum(transvalue) from fintrans where fintrans.acctno=acct.acctno)
	where acctno=@acctno
	
	exec DbArrearsCalc   @acctno=@acctno,@countpcent=0,@nodates=0,@arrears=0,@return=@return
	--SELECT @paid AS prevpaid,@amtpaid AS amt for testing
	update acct 
	SET lastupdatedby=@machid,
	paidpcent=-isnull((@amtPaid+@paid)/agrmttotal*100,0),
	datelastpaid=@transdate
	where acctno=@acctno AND agrmttotal >0 
	
	if SUBSTRING(@acctno,4,1)='4' and exists
	(
		select acctno 
		from acct 
		where acctno=@acctno 
			and paidpcent=100 
			and @paymethod!=2
	)
	--72363 must check that no cheque payment made in last x days
	and not exists
	(
		select acctno
		from fintrans
		where acctno = @acctno
		and transtypecode = 'PAY'
		and paymethod = 2
		and datetrans > dateadd(d, -@chqdays, getdate())
	)
	begin
		exec DN_ProposalClearSP @acctno=@acctno,@empeeno=@machid,@source = 'Auto',@return=@return --IP - 15/04/10 - UAT(91) UAT5.2 - Added Source for DA
	end
	
	END
	
END 
delete  from AccountLocking
where acctno=@acctno
and lockedby=@machid
set @return = 1

-- Loyalty

IF EXISTS (SELECT * FROM loyalty
           WHERE loyaltyacct = @acctno) AND (SELECT SUM(transvalue) FROM fintrans 
                                             WHERE acctno = @acctno) = 0
BEGIN
	UPDATE loyalty
	SET statusacct = 1
	WHERE statusacct = 4
	AND loyaltyacct = @acctno
END

--IP - 29/09/10 - CR1202 - Return the current balance and fee applied
set @outstbal = (select outstbal from acct where acctno = @acctno) 
select @outstbal as 'Current Account Balance'

select @collectionFEE as 'Actual Fee Applied'

end try
begin catch
set @errmsg = ERROR_MESSAGE()
PRINT @errmsg 
set @return = 0

IF @@TRANCOUNT >0 
	rollback tran

update CSKTransactions
set errormessage=@errmsg
where acctno=@acctno
and transdate=@transdate

END CATCH
IF @@TRANCOUNT >0 
	COMMIT
go
