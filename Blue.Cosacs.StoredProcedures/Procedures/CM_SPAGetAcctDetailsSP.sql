SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SPAGetAcctDetailsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SPAGetAcctDetailsSP]
GO
-- =================================================================================================
-- Author:		Ilyas Parker
-- Create date: 29/09/2008
-- Description:	This procedure will return the 'Outstanding Balance', 'Arrears', and 
--				'Monthly Instalment' for an account to display on the 'Special Arrangements' screen.	
--				If this is an RF account, then the instalment amounts will be consolidated.	
-- Change Control
-- --------------
-- 08/01/09 jec CR976 additional parameters
-- 08/08/09 jec correct datecheck on intratehistory
-- 29/04/10 ip  UAT(983) UAT5.2 - Return ServPCent InsPcent and AdminPcent based on todays date and
--				the customers last proposal. Return Rebate.
-- 22/09/10 ip  UAT5.2 Log - UAT(1017) - Special Arrangements - Extended Term - Minimum Term for Terms Type
-- 23/09/10 ip	UAT5.2 Log - UAT(1017) - Return the Score Band of the account (to be used for Serial 2 account for Extended Term)
-- 24/09/10 ip  UAT5.2 Log - UAT(1017) - Set ScoreBand to scoreband from intratehistory - for serial 2 account for Extended Term
-- =================================================================================================

CREATE PROCEDURE [dbo].[CM_SPAGetAcctDetailsSP] 
                @acctno varchar(12),
				@outstbal money OUTPUT,
				@arrears money OUTPUT,
				@instalamount money OUTPUT,
				@dateacctopen datetime OUTPUT,		-- CR976
				@percentpaid int OUTPUT,			-- CR976
				@finalpaydate datetime OUTPUT,		-- CR976
				@type char(1) OUTPUT,				-- CR976
				@term int output,
				@maxterm int output,
				@currinstno int output,
				@termstype char(2) output,
				@refindeposit money OUTPUT,
				@servpcent money output,
				@cashprice money output,
				@dueday int output,
				@inspcent MONEY OUTPUT,				--IP - 29/04/10 - UAT(983) UAT5.2
				@adminpcent MONEY OUTPUT,			--IP - 29/04/10 - UAT(983) UAT5.2
				@rebate	MONEY OUTPUT,				--IP - 30/04/10 - UAT(983) UAT5.2
				@minterm INT OUTPUT,				--IP - 22/09/10 - UAT(1017)UAT5.2
				@scoringBand VARCHAR(1) OUTPUT,			--IP - 23/09/10 - UAT(1017)UAT5.2
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

	--declare @type varchar(3) --Store the account type
	declare @custid varchar(20) --Store the Customer ID of the account
	declare @totagrmt money, @totoutstbal money		-- CR976
	declare @payments money
	
	create table #AcctDetails
	(
		outstbal money,
		arrears money,
		instalamount money,
		dateacctopen datetime,		-- CR976
		percentpaid money,			-- CR976
		finalpaydate datetime,		-- CR976
		term int,
		maxterm	int,
		currinstal int,
		termstype char(2),
		refindeposit money,
		ServPcent money,
		CashPrice money,
		dueday INT,
		InsPcent MONEY,				--IP - 29/04/10 - UAT(983) UAT5.2
		AdminPcent MONEY,			--IP - 29/04/10 - UAT(983) UAT5.2
		MinTerm	INT,				--IP - 22/09/10 - UAT(1017)UAT5.2
		ScoringBand VARCHAR(1)			--IP - 23/09/10 - UAT(1017)UAT5.2
	)
	
    set @return = 0    --initialise return code

	--Select the type of the account
	set @type = (select accttype from acct where acctno = @acctno)
	set @custid = (select custid from custacct where acctno = @acctno and hldorjnt = 'H')
	
	--if(@type = 'R')
	--	begin			
			insert into #AcctDetails
			select a.outstbal,a.arrears,i.instalamount,null,a.paidpcent,null,0,0,0,' ',0,0,ag.cashprice,i.dueday,0,0,0,''	-- CR976	--IP - 29/04/10 - UAT(983)UAT5.2 --IP - 22/09/10 - UAT(1017)UAT5.2 - Added MinTerm	--IP - 23/09/10 - UAT(1017)UAT5.2 - Added ScoringBand
			from acct a INNER JOIN instalplan i on a.acctno = i.acctno
						INNER JOIN agreement ag on a.acctno = ag.acctno
			where a.acctno = @acctno
			
			update #AcctDetails	
			set	dateacctopen = (select a.dateacctopen
							from acct a 
							where acctno=@acctno),			
				-- final paydate includes months in arrears			
				finalpaydate = (select dateadd(m,cast((a.arrears/i.instalamount) as int),i.datelast)
							from instalplan i inner join acct a on
								i.acctno = a.acctno 
							where a.acctno=@acctno)
								
	-- Get rebate for calc of refinace deposit
	declare 	
	@poRebate money,
	@poRebateWithin12Mths money,
	@poRebateAfter12Mths money	

	exec dbo.DN_RebateSP @AcctNo ,@poRebate OUTPUT,	@poRebateWithin12Mths OUTPUT,@poRebateAfter12Mths OUTPUT,
		@return OUTPUT, @FromDate= '01-jan-1900',@FromThresDate= '01-jan-1900',
		@UntilThresDate	   = '01-jan-1900',@RuleChangeDate = '01-apr-2002',@RebateDate= '01-jan-1900'
	
	-- get amount paid (excluding charges)
	select @payments=ABS(sum(transvalue)) from fintrans 
		where acctno=@AcctNo and transtypecode in('PAY','COR','REF','RET','SCX','REB','XFR')
	
			
	-- term
	--IP - 29/04/10 - UAT(983) UAT5.2
	DECLARE @maxDateProp DATETIME 
	
	SET @maxDateProp = (select MAX(dateprop) FROM proposal WHERE custid = @custid)
	
	update #AcctDetails
	set term = instalno, maxterm=t.maxterm, MinTerm = t.minterm, currinstal= datediff(m,i.datefirst,getdate()),	--IP - 22/09/10 - UAT(1017)UAT5.2 - Added MinTerm
		termstype=a.termstype, refindeposit= case
--				when @payments-(ag.servicechg-@poRebate) < 0 then 0
--				else @payments-(ag.servicechg-@poRebate)
				when (a.agrmttotal-a.outstbal)-(ag.servicechg-@poRebate) < 0 then 0
				else (a.agrmttotal-a.outstbal)-(ag.servicechg-@poRebate)
				end,
		--ServPcent=t.ServPcent
		ServPcent=r.intrate,			--IP - 29/04/10 - UAT(983) UAT5.2
		InsPcent = CASE WHEN r.insincluded = 0 THEN r.inspcent ELSE 0 END,		--IP - 04/05/10 - UAT(983) UAT5.2
		AdminPcent = r.AdminPcent,
		--ScoringBand = CASE WHEN ISNULL(p.ScoringBand,'') = '' THEN r.Band ELSE ISNULL(p.ScoringBand,'') END	--IP - 23/09/10 - UAT(1017)UAT5.2 --IP - 24/09/10 - UAT(1017) 
		ScoringBand = r.Band --IP - 24/09/10 - UAT(1017) UAT5.2
	--from instalplan i inner join acct a on i.acctno = a.acctno
	--			INNER JOIN termstype t on a.termstype=t.termstype
	--			INNER JOIN agreement ag on a.acctno=ag.acctno
	--			INNER JOIN proposal p on ag.acctno = p.acctno,
	--			intratehistory r 
	--				where a.acctno=@acctno
	--				and p.dateprop > r.datefrom and (p.dateprop<=r.dateto or r.dateto='1900-01-01')		-- jec 08/08/09
	--				and r.termstype=a.termstype 
	--				and (p.points between r.PointsFrom and r.PointsTo or r.PointsTo=0)
	
	--IP - 29/04/10 - UAT(983) UAT 5.2 - Return the InsPcent and AdminPcent
	from instalplan i inner join acct a on i.acctno = a.acctno
				INNER JOIN termstype t on a.termstype=t.termstype
				INNER JOIN agreement ag on a.acctno=ag.acctno,
				--INNER JOIN proposal p on ag.acctno = p.acctno,
				intratehistory r ,
				proposal p
					where a.acctno=@acctno
					AND p.custid = @custid
					AND p.dateprop = @maxdateprop
					and GETDATE() > r.datefrom and (GETDATE()<=r.dateto or r.dateto='1900-01-01')		-- jec 08/08/09
					and r.termstype=a.termstype 
					--and (p.points between r.PointsFrom and r.PointsTo or r.PointsTo=0)
					--AND ISNULL(p.scoringband, '') = r.Band
					AND (ISNULL(p.ScoringBand, '') = r.Band OR (r.PointsTo = 0 OR p.points BETWEEN r.PointsFrom AND r.PointsTo) )
						

	--Return the values as out parameters
	set @outstbal = (select outstbal from #AcctDetails)
	set @arrears = (select arrears from #AcctDetails)
	set @instalamount = (select instalamount from #AcctDetails)
	select @dateacctopen = dateacctopen from #AcctDetails
	select @percentpaid = percentpaid from #AcctDetails
	select @finalpaydate = finalpaydate from #AcctDetails
	select @term=term from #AcctDetails
	select @maxterm=maxterm from #AcctDetails
	select @currinstno=currinstal from #AcctDetails
	select @termstype=termstype from #AcctDetails
	select @refindeposit=refindeposit from #AcctDetails
	select @servpcent=servpcent from #AcctDetails
	select @cashprice=cashprice from #AcctDetails
	select @dueday=dueday from #AcctDetails
	SELECT @inspcent = InsPcent FROM #AcctDetails				--IP - 29/04/10 - UAT(983)UAT5.2
	SELECT @adminpcent = AdminPcent FROM #AcctDetails			--IP - 29/04/10 - UAT(983)UAT5.2
	SELECT @rebate = @poRebate									--IP - 30/04/10 - UAT(983)UAT5.2
	SELECT @MinTerm = MinTerm FROM #AcctDetails					--IP - 22/09/10 - UAT(1017)UAT5.2
	SELECT @ScoringBand = ScoringBand FROM #AcctDetails			--IP - 23/09/10 - UAT(1017)UAT5.2 

	--select * from #AcctDetails
	--select @outstbal, @arrears, @instalamount

    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
