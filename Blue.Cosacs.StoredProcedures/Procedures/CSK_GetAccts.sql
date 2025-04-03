
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CSKGetAccts') 
DROP PROCEDURE CSKGetAccts
GO 
CREATE PROCEDURE CSKGetAccts 
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : CSKGetAccts.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure to return account details to the Kiosk
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/06/10	 IP  Creation.
-- 29/09/10  IP  CR1202 - Generate collection FEES for payments

************************************************************************************************************/
@qryType SMALLINT, -- 1 = custid, 2 = acctno, 3=passport 
@refno VARCHAR(20) ,
@machid int,
@return int output
AS
Begin transaction
SET NOCOUNT ON -- marginally improves performance
begin try

SET NOCOUNT ON 
declare @errmsg varchar(2000)
DECLARE @accts TABLE (acctno CHAR(12) PRIMARY KEY ,custid VARCHAR(20)) 

delete from AccountLocking where lockedby=@machid

IF @qryType IN ( 1 ,3) -- custid or passport 
BEGIN
	INSERT INTO @accts (acctno, custid  )
	SELECT a.acctno ,ca.custid FROM custacct ca, acct a 
	WHERE ca.custid = @refno AND ca.hldorjnt = 'H'
	AND a.agrmttotal >0 AND a.currstatus!='S' 
	 AND ca.acctno= a.acctno 
	 and not exists (select 'x' from AccountLocking x where x.acctno=a.acctno and x.lockcount>=1
	 AND lockedby !=@machid )
END	
ELSE IF @qryType =2 -- acctno 
BEGIN
	INSERT INTO @accts (acctno, custid  )
	SELECT a.acctno ,ca.custid FROM custacct ca, acct a 
	WHERE ca.acctno = @refno AND ca.hldorjnt = 'H'
	AND a.agrmttotal >0 AND a.currstatus!='S' 
	 AND ca.acctno= a.acctno 
	 and not exists (select 'x' from AccountLocking x where x.acctno=a.acctno and x.lockcount>=1
	 AND lockedby !=@machid )
END


insert into AccountLocking
(acctno, lockedby,lockedat,lockcount)
select acctno, @machid,GETDATE(),1
from @accts

	SELECT a.acctno , 
	ca.custid ,
	cu.NAME ,
	a.accttype AS AcctType,
	a.dateacctopen AS DateOpened,
	a.agrmttotal AS AgreementTotal, 
	ISNULL(a.outstbal,0) AS Balance, 
	ISNULL(a.arrears,0) AS Arrears, 
	a.currstatus AS CurrentStatus, 
	a.highststatus AS HighestStatus,
	CONVERT(MONEY,0) AS AmountPaid, 
	a.paidpcent AS PercentagePaid,
	a.datelastpaid ,
	a.termstype,
	CONVERT(MONEY,0) AS rebate ,
	CONVERT(MONEY,0) AS  EarlySettle,
	i.datelast, 
	i.instalamount AS instalment ,
	i.fininstalamt AS FinalInstalment, 
	cu.rfcreditlimit AS Creditlimit,
	cu.availablespend AS AvailableSpend, 
	ag.datenextdue AS DueDate,
	i.DATEFIRST, 
	i.instalno ,
	ag.datedel ,
	ag.deposit,
	ag.servicechg AS ServiceCharge,
	CONVERT(VARCHAR(32),'') AS Segment    ,
	CONVERT(MONEY,0) AS AddtoPotential, 
	CONVERT(CHAR(1),'N') AS FinalPayment,
	CONVERT(MONEY,0) AS CollectionFee				--IP - 28/09/10  - CR1202
	INTO #Accts
	FROM @accts ca 
	JOIN customer cu ON cu.custid = ca.custid 
	JOIN acct a ON a.acctno= ca.acctno 
	LEFT JOIN instalplan i ON i.acctno= ca.acctno 
	JOIN agreement ag ON ag.acctno= a.acctno 
	AND a.currstatus != 'S'
	
	-- TODO need to exclude accounts locked by Cosacs - need input parameter of CSK terminal

	declare @acctno char(12),@porebate MONEY, @addto MONEY , @accttype CHAR(1), @arrears money, @collectionFEE MONEY --IP - 28/09/10 CR1202 - Added @collectionFEE
	declare acct_cursor CURSOR FAST_FORWARD READ_ONLY FOR
	SELECT acctno ,AcctType
	FROM #accts  
	OPEN acct_cursor
	FETCH NEXT FROM acct_cursor INTO @acctno,@accttype
	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec dbo.DN_RebateSP @AcctNo ,@poRebate = @poRebate OUTPUT,
			@return =0, @FromDate= '01-jan-1900',@FromThresDate= '01-jan-1900',
			@UntilThresDate	   = '01-jan-1900',@RuleChangeDate = '01-apr-2002',@RebateDate= '01-jan-1900'
			
		exec DbArrearsCalc   @acctno=@acctno,@countpcent=0,@nodates=0,@arrears=@arrears output,@return=@return output
		
		EXEC CSK_CalculateCollectionFEESP @acctno, @arrears, 0, @collectionFEE OUTPUT, @return	--IP - 28/09/10 - CR1202
	
		UPDATE #Accts SET rebate = @porebate,EarlySettle = Balance -isnull(@porebate,0) + @collectionFEE, arrears=@arrears   WHERE acctno= @acctno --IP - 29/09/10 - CR1202 - Added CollectionFee to EarlySettlement.
		 
		IF @accttype NOT IN ('R','C','S') -- calculate addto 
		BEGIN
				EXEC DN_AddToCalcSP
				@acctno = @acctno, 
				@rebate = @POrebate, 
				@value = @addto OUTPUT ,
				@return =0
				UPDATE #Accts SET AddtoPotential = @addto WHERE acctno = @acctno 
		END	
		
		UPDATE #Accts SET CollectionFEE = @collectionFee WHERE acctno = @acctno	--IP - 28/09/10 - CR1202
			
	FETCH NEXT FROM acct_cursor INTO @acctno,@accttype
	END
	CLOSE acct_cursor
	DEALLOCATE acct_cursor
	-- updating here as arrears calc would have recalculated datenext due
	UPDATE #Accts SET duedate = g.datenextdue FROM agreement g 
		WHERE g.acctno=#Accts.acctno 
	
	UPDATE #Accts 
	SET Segment = t.Segment_Name
	FROM TM_Segments t 
	WHERE t.Account_Number= #Accts.acctno 

	UPDATE #Accts SET FinalPayment = 'Y' WHERE  EarlySettle <= instalment AND acctno LIKE '___0%'
	AND EarlySettle >0
	
	UPDATE #Accts 
	    SET  AmountPaid= -isnull((SELECT sum(transvalue) from fintrans 
		where  transtypecode in('PAY','COR','REF','RET','SCX','REB','XFR')
		AND fintrans.acctno = #Accts.acctno),0)
	
	delete from csk_get_audit where acctno=@refno or custid=@refno
	
	update #accts set balance=agreementtotal+amountpaid, earlysettle=agreementtotal+amountpaid where SUBSTRING(acctno,4,1)='4'
		
		
	-- just in case anyone messes with the columns in csk this transaction will still work. 	
	INSERT INTO csk_get_audit (
		acctno,	custid,	[NAME],	AcctType,	DateOpened,
		AgreementTotal,	Balance,	Arrears,	CurrentStatus,	HighestStatus,
		AmountPaid,	PercentagePaid,	datelastpaid,	termstype,	rebate,
		EarlySettle,	datelast,	instalment,	FinalInstalment,	Creditlimit,
		AvailableSpend,	DueDate,	[DATEFIRST],	instalno,	datedel,
		deposit,	ServiceCharge,	Segment,	AddtoPotential,	FinalPayment, CollectionFee) --IP - 28/09/10 - CR1202
	SELECT 

		acctno,	custid,	[NAME],	AcctType,	DateOpened,
		AgreementTotal,	Balance,	Arrears,	CurrentStatus,	HighestStatus,
		AmountPaid,	PercentagePaid,	datelastpaid,	termstype,	rebate,
		EarlySettle,	datelast,	instalment,	FinalInstalment,	Creditlimit,
		AvailableSpend,	DueDate,	[DATEFIRST],	instalno,	datedel,
		deposit,	ServiceCharge,	Segment,	AddtoPotential,	FinalPayment, CollectionFee --IP - 28/09/10 - CR1202
	FROM 	 #accts
	
	if @@ROWCOUNT=0
	BEGIN
		set @return=0
	END
	ELSE
	BEGIN
		set @return=1
		SELECT * FROM #Accts
	END
	
	end try
	begin catch
	set @errmsg = ERROR_MESSAGE()
	PRINT @errmsg
	set @return = 0
	IF @@TRANCOUNT >0 
		ROLLBACK 
	end CATCH
	
	IF @@TRANCOUNT >0
		commit;
	
GO 
	
