SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetRFCombinedForPrintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetRFCombinedForPrintSP]
GO


CREATE PROCEDURE dbo.DN_CustomerGetRFCombinedForPrintSP
			@custid varchar(20),
			@available_credit money OUT,
			@cardprinted char(1) OUT,
			@total_agreements money OUT,
			@total_arrears money OUT,
			@total_balances money OUT,
			@total_credit money OUT,
			@total_delivered_instalments money OUT,
			@total_all_instalments money OUT,
			
			@return int OUTPUT

AS
		SET 	@return = 0			--initialise return code

		SELECT	@total_credit = RFCreditLimit,
			 	@cardprinted = RFCardPrinted
		FROM 	 	customer
		WHERE	custid =@custid

		SET @return =@@error
   
   	IF @return = 0
		BEGIN
			SELECT	@total_agreements=isnull(sum(acct.agrmttotal),0),
			       		@total_balances = isnull(sum(acct.outstbal),0),
			       		@total_arrears =isnull(sum(acct.arrears),0),
			       		@total_delivered_instalments =isnull(sum(instalplan.instalamount),0)
			FROM  		custacct
      					join acct on acct.acctno = custacct.acctno
					join instalplan on instalplan.acctno =acct.acctno
					join agreement on agreement.acctno =acct.acctno
			WHERE 	custid =@custid 
			AND 		acct.accttype =N'R'
			AND 		custacct.hldorjnt=N'H'
			AND 		acct.outstbal != 0    -- DSR Need to include credit balances
         and agreement.deliveryflag = 'Y'
			
			SET @return =@@error
		END
	IF @return = 0
		BEGIN
			SELECT	@total_all_instalments=isnull(sum(instalplan.instalamount),0)
			     
			FROM  		custacct
      					join acct on acct.acctno = custacct.acctno
					join instalplan on instalplan.acctno =acct.acctno
					join agreement on agreement.acctno =acct.acctno
			WHERE 	custid =@custid 
			AND 		acct.accttype =N'R' 
			AND 		custacct.hldorjnt=N'H'
			And      acct.currstatus !=N'S'
                                       --and agreement.deliveryflag = 'Y' 
         --removed delivery flag condition so that this account would appear on the ready finance printout
         --am sure I have reinstated this before, cannot remember why-we may need a new variable to cater for different circumstances
			SET @return =@@error
		END

		IF @return = 0
		BEGIN
        	EXECUTE  DN_CustomerGetRFLimitSP	
        			 @custid = @custid,
        			 @AcctList = '',
                 	 @limit = @total_credit output,
                 	 @available = @available_credit output,
                 	 @return = @return output
			SET @return =@@error
		END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

