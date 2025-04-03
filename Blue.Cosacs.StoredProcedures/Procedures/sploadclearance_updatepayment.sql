SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sploadclearance_updatepayment]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sploadclearance_updatepayment]
GO

CREATE procedure sploadclearance_updatepayment
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : sploadclearance_updatepayment.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DA screen - Check for payment made
-- Author       : ??
-- Date         : ??
--
-- This procedure will Check payment has been made when DA.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 29/05/12  jec #10246 Payment Outstanding pop up comes up in the DA screen even after paying the first instalment.
-- =================================================================================
	-- Add the parameters for the stored procedure here
as
begin
/*
    Procedure determines whether customer has paid enough to clear delivery authorisation.
    Cash accounts need to either paid for balance OR if they are cash on delivery 
    and it is Singapore they need to pay 10 percent of the agreement total. 
    HP accounts need to have paid their deposit.
    in both cases if the customer has paid by cheque cheque is not considered cleared 
    for a few days (country parameter)
*/

    declare @cheqdays smallint,
            @countrycode varchar (2),
            @status integer,
            @query_text varchar (256),
            @codpercentage float

/* Get no of days set to cheque clearance from the country table */

    select @cheqdays = cheqdays
    from   country

    set @status = @@error

    if @status = 0
    begin
        /* Mark payment outstanding where a deposit is required or there  */
        /* is a normal cash acct with a non-zero agreement total.         */
    update 	#temp_acct1 
	set 	paymentos = 1 
	where 	(deposit > 0 
	or	(accttype = 'C' and agrmttotal > 0 and CODFlag = 'N' ))
	and 	accttype != 'L'
       set @status = @@error
    end
    

    if @status = 0
    begin   
        /* Sum amount already paid on each account from FinTrans        */
        /* Cash is cleared immediately and cheques require n 'cheqdays' */
       /* AA-amending cheque clearance days to be negative*/
        update 	#temp_acct1
        set 	alreadypaid = 
                (select isnull(sum(-transvalue), 0)
                 from   fintrans f
                 where  transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' )
	 	 and 	((paymethod%10) != 2 or  ((paymethod%10) = 2 and dateadd (day, @cheqdays, datetrans) < getdate()))
	 	 -- changed as surely should add cheque days onto the payment and this would make it negative.
                         /*exclude cheque payments*--*/
                 and 	f.acctno = #temp_acct1.acctno)
        set @status =@@error
    end


    if @status = 0
    begin
        /* Mark cheque outstanding for accounts with cheques waiting to clear */
        update 	#temp_acct1
        set 	chequeos = 1 
	where 	acctno in 
                (select acctno 
		 from   fintrans f
        	 where  transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' )
                 and 	((paymethod%10) = 2 and dateadd (day,@cheqdays, datetrans) > getdate())
      			/*exclude cheque payments*--*/
        	 and 	f.acctno = #temp_acct1.acctno)
        set @status =@@error
    end


    if @status = 0
    begin
        /* Cheque is not outstanding if non-cash and deposit was paid */
        /* or normal cash acct and agreement total has been paid.     */
    	update 	#temp_acct1
        set 	chequeos = 0 
		where (alreadypaid >= deposit and accttype !=N'C' and accttype!=N'L' )
                   or (alreadypaid >= agrmttotal and accttype = 'C' and CODFlag = 'N' )
        set @status =@@error
    end

    if @status = 0
    begin
        /* Payment is not outstanding on non-cash accounts if the   */
        /* sum of transactions on FinTrans is at least the deposit. */
        update 	#temp_acct1
        set	paymentos = 0 
	where 	(select isnull (sum (-transvalue),0)
        	from 	fintrans f
        	where 	transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' )
                and     f.acctno = #temp_acct1.acctno) >= deposit 
        and accttype !=N'C'
	and accttype !=N'L'
        set @status = @@error	
    end

    if @status = 0
    begin
        /* Payment is not outstanding on cash accounts if the   */
        /* sum of transactions on FinTrans equal to agreement total. */
        update 	#temp_acct1
        set		paymentos = 0 
        where 	(select isnull (sum (-transvalue),0) + .01
        from 	fintrans f
        where 	transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' )
        and     	f.acctno = #temp_acct1.acctno) >= agrmttotal 
        and 	(accttype = 'C' or accttype = 'L')
        and	chequeos = 0 
      set @status = @@error	
    end

    /* Payment outstanding on COD cash accounts if    */
    /* percentage paid < country parameter codpercentage. */
    if @status = 0
    begin
        select @codpercentage = codpercentage
        from   country
	
        update	#temp_acct1
        set    	paymentos= 1 
        where	((select (isnull(sum (-transvalue), 0)/ agrmttotal)*100
	       	 from   fintrans f, country c
	 	 where transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' )
	 	 and f.acctno = #temp_acct1.acctno) < @codpercentage or #temp_acct1.chequeos = 1)
        and accttype = 'C'
        and CODFlag= 'Y' AND agrmttotal >0
        set @status = @@error 
    end

   select @countrycode = countrycode
    from   country

	if @status = 0
        begin
            /* if outstanding flags don't attempt to AUTO DA IC*/
            update #temp_acct1
            set    paymentos= 1     
			where  custid in (select custid from instantcreditflag where nullif(datecleared, '1900-01-01') is null)
					and accttype='R'			-- #10246
            set @status =@@error
        end

/* For Singapore only */

/* Get COD accounts with out 10% of payment */

    if @countrycode= 'S'
    begin
        begin
            /* Mark payment outstanding on COD account if sum of transactions */
            /* on FinTrans are less than 10% of the agreement total.          */
            update #temp_acct1
            set    paymentos= 1 
	    where  (select isnull (sum (-transvalue), 0)
            	    from   fintrans f
            	    where transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' )
            and f.acctno = #temp_acct1.acctno) < agrmttotal/10
            and accttype = 'C'
            and CODFlag= 'Y' and agrmttotal >0
            set @status = @@error
        end
        if @status = 0
        begin
            /* Extra check to mark payment outstanding if there are */
            /* no transactions on FinTrans.                         */
            update #temp_acct1
            set    paymentos= 1     
	    where  accttype = 'C'
            and    CODFlag= 'Y'
            and    acctno not in (select acctno from fintrans
                   where transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX' ))
            set @status =@@error
        end

		
    end

    set @status =@@error

    return @status

end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

