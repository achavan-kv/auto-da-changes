SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetRFCombined]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetRFCombined]
GO



CREATE
 procedure DN_CustomerGetRFCombined 
         @custid varchar(20)
as
/*this is used for account details in Cosacs I had to change this has OpenROAD does not use byref very well
through stored procedures*/
declare  @available_credit money,
         @Cardprinted char(1) ,
         @total_agreements money,
         @total_arrears money ,
         @total_balances money,
         @total_credit money ,
         @total_instalments money ,
         @status int 

   select 
   @total_credit =RFCreditLimit,
   @cardprinted =RFCardPrinted
   from 
       customer
   where
   custid =@custid
   set @status =@@error
   
   if @status = 0
   begin
      select @total_balances = isnull(sum(acct.outstbal),0),
             @total_arrears =isnull(sum(acct.arrears),0),
             @total_instalments =isnull(sum(instalplan.instalamount),0)
      from  custacct
      join acct on acct.acctno = custacct.acctno
      join instalplan on instalplan.acctno =acct.acctno
      join agreement on agreement.acctno =acct.acctno
      where custid =@custid and
      acct.accttype not in ('C','S') and custacct.hldorjnt=N'H'
      and agreement.deliveryflag =N'Y'
      and acct.outstbal > 0
      set @status =@@error

   end
  if @status = 0
   begin
      select @total_agreements=isnull(sum(acct.agrmttotal),0)
      from  custacct
      join acct on acct.acctno = custacct.acctno
      where custid =@custid and
      acct.accttype =N'R' and custacct.hldorjnt=N'H'
      and acct.currstatus !=N'S'
      set @status =@@error
   
   end
   if @status = 0
   begin
        execute  DN_CustomerGetRFLimitSP
                 @custid = @custid,
                 @AcctList = '',
                 @limit = @total_credit output,
                 @available = @available_credit output,
                 @return = @status output
  end

select    isnull(@available_credit,0) as available_credit,
          isnull( @Cardprinted,'') as cardprinted,
          isnull(@total_agreements,0) as total_agreements,
          isnull(@total_arrears,0) as total_arrears,
          isnull(@total_balances,0) as total_balances,
          isnull(@total_credit,0) as total_credit,
          isnull(@total_instalments,0) as total_instalments,
         @status as status


  return @status
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

