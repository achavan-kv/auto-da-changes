SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransSumAccountGetSP]')
	  and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransSumAccountGetSP]
GO

CREATE procedure DN_FintransSumAccountGetSP 
--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_FintransSumAccountGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Sum Fintrans transactions by type
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
-- 22/04/09  jec CR976 Refinance Arrangements - cater for RFN/RFD transtypes
-- 26/05/10  ip  UAT(232)UAT5.2.1.0 Log - Exclude Home Club Voucher value from the Delivery Total
-- 11/10/11  jec #3902 CR1232 disbursement transaction type
--------------------------------------------------------------------------------

    -- Parameters 
@acctNo varchar(12),
@return int OUTPUT
as
declare
   @errortext varchar(128),
    @amountpaid money,
    @deliverytotal money,
    @tofollow money ,
    @rebate money,
    @addtopotential money,
    @lastmovement smalldatetime,
    @totalfees money,
    @earlysettlement money ,
    @agrmttotal money,
    @balance money,
    @totalbailfees money,
    @warrantyadjustment money,
	@isAmortized bit
begin

    set @errortext = 'getting amount paid for account from fintrans '

	EXEC	DN_FintransGetAmountPaidSP @acctno, @amountpaid OUT, @return OUT
	EXEC	DN_FintransGetWarrantyAdjustmentSP @acctno, @warrantyadjustment OUT, @return OUT	

/*
    select @amountpaid = isnull(sum (transvalue),0) from fintrans where acctno =@acctno
    and transtypecode in ('PAY','COR','XFR','JLX','SCX','REF','RET','DDE','DDN','DDR')
*/

    if @return =0
    begin
        set @errortext = 'getting agreement total needed for to follow '
        select @agrmttotal=isnull(agrmttotal,0),
               @balance =isnull(outstbal,0)
        from acct where acctno =@acctno
        set @return= @@error
    end

    if @return =0
    begin
        set @errortext = 'getting delivery total for account from fintrans '
        select @deliverytotal  = isnull(sum(transvalue),0) from fintrans where acctno =@acctno
        and transtypecode in ('DEL', 'GRT', 'ADD', 'REP', 'RDL','RFN','CLD') -- jec CR1232 
        set @return= @@error
        -- include RFD trans with Debit balance
        select @deliverytotal  = @deliverytotal +isnull(sum(transvalue),0) from fintrans where acctno =@acctno
        and transtypecode in ('RFD') and transvalue>0 -- jec CR976 31/03/09 added RFD Refinance Deposit     
        
        --IP - 26/05/10 - UAT(232) UAT5.2.1.0 Log 
        SELECT @deliverytotal = @deliverytotal - ISNULL(transvalue,0) FROM delivery WHERE acctno = @acctno AND itemno = 'ZXHC'
         			
        set @return= @@error
        set @tofollow =@agrmttotal-@deliverytotal
		
		select @isAmortized=isAmortized from acct where acctno=@acctNo
		IF(@isAmortized = 1)
		BEGIN
			IF EXISTS (SELECT * FROM cashloan WHERE AcctNo = @acctNo and LoanStatus!='D')
			BEGIN
				 SET @tofollow = @agrmttotal
			END
			IF EXISTS (SELECT * FROM cashloan WHERE AcctNo = @acctNo and LoanStatus='D')
			BEGIN
				 SET @tofollow = 0 
			END
		END
		
        IF EXISTS (SELECT * FROM Acct_Archive WHERE acctno = @acctNo)
        BEGIN
		     SET @tofollow = 0 -- assume archived account fully delivered. Archive UAT issue 10	
		  END
    end
    if @return =0
    begin
       set @errortext = 'getting fees total for account from fintrans '
       select  @totalfees  = isnull(sum(transvalue),0) from fintrans where acctno =@acctno
       and transtypecode in ('INT', 'FEE', 'ADM')
       set @return= @@error
    end
    if @return =0
    begin
       set @errortext = 'getting fees total for account from fintrans '
       select  @totalbailfees  = isnull(sum(transvalue),0) from fintrans where acctno =@acctno
       and transtypecode = 'FEE'
       set @return= @@error
    end
    if @return =0
    begin
       set @errortext = ' do rebate calculation here'
       set @rebate = 0
       set @return= @@error
    end

    if @return =0
    begin
       set @errortext = 'doing add to potential calculation '
       execute @return= dbaddtocalc @acctno=  @acctno,
                           @value = @addtopotential output,
                           @rebate =@rebate
	   IF(@isAmortized = 1)
	   BEGIN
			IF EXISTS (SELECT * FROM cashloan WHERE AcctNo = @acctNo and LoanStatus='D')
			BEGIN
				 SET @rebate = 0 
			END
	   END
    end
    if @return =0
    begin
       set @errortext =N'doing last movement '
       select @lastmovement = isnull(Max(datetrans),'1-jan-1900') from
       fintrans
       where
       acctno =@acctno
    end
    if @return =0
    begin
        set @earlysettlement=@balance-@rebate

        select   --isnull(@amountpaid,0) + isnull(@warrantyadjustment,0) as 'Amount Paid', /*Issue 5 Internal UAT 5.1*/
                 isnull(@amountpaid,0) as 'Amount Paid',
				 isnull(@deliverytotal,0) as 'Delivery Total',
				 isnull(@tofollow, 0) as 'To Follow',
				 isnull(@rebate,0) as 'Rebate',
				 isnull(@addtopotential,0) as 'Add-to Potential',
				 @lastmovement as 'Date Last Movement',
				 isnull(@totalfees,0) as 'Total Fees',
				 isnull(@totalbailfees,0) as 'Total Bail Fees',
				 isnull(@earlysettlement,0) as 'Early Settlement',
				 isnull(@warrantyadjustment,0) as 'Warranty Adjustment'  /*IP - 27/11/2007*/
    end    


end 
GO
grant execute on DN_FintransSumAccountGetSP to public
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
