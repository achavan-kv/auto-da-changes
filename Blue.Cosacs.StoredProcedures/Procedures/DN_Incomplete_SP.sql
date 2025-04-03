if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_Incomplete_SP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_Incomplete_SP]
GO


CREATE procedure  DN_Incomplete_SP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_Incomplete_SP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Incomplete Credit Applications
-- Date         : ??
--
-- This procedure will load the Incomplete Credit Applications.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 01/06/11 jec  #3742 Ensure correct data returned
-- 03/06/11 jec  #3741 performance issue
-- 24/10/11 IP   #3896 - CR1232 - New filter for Customers qualified for Cash Loan that have referred accounts.
-- 05/12/11 IP   #8777 - LW73607 - When searching on account number previously, the correct branch that the account
--				 belonged to had to be selected from the branch drop down. Now should be able to search on just 
--				 the account number
-- 10/01/12 IP   #3780 - Re-instated LEFT JOIN as for new applications where Sanction Stage 1 not completed Proposal.Reason is null.
-- 24/02/12 IP   #9598	- UAT 87 - Display Referral Reasons for a referred Cash Loan account
-- ================================================
	-- Add the parameters for the stored procedure here
         @branchrestriction  varchar (5),
         @holdflags  varchar(6),
         @viewLimit varchar(6),
         @return integer output,
         @acctno varchar (12),
         @Singleflagonly bit,
         @excludenoitems bit,
         @excludeUnpaid bit,
         @refcode varchar(2),
         @ReferralCL BIT,
         @PendingCL BIT
 
AS
       set nocount on
       
       declare @cashLoanQual bit													--IP - 24/10/11 - #3896 - CR1232
	   set @cashLoanQual = 0														--IP - 24/10/11 - #3896 - CR1232

     set @return = 0  
   if @holdflags= '%'
   begin
      set @holdflags = NULL 
   end
   else if @holdflags = 'RL'														--IP - 24/10/11 - #3896 - CR1232
   begin
      set @holdflags = 'R'
      set @cashLoanQual = 1
   end

  if @refcode= ''
      set @refcode = NULL 
   
	  if @refcode= '%'
      set @refcode = NULL
      
        IF SUBSTRING(@branchrestriction,LEN(@branchrestriction),1) != '%'
			SET @branchrestriction = @branchrestriction + '0%'
   
   --DELETE d   
   --FROM delauthorise d
   --INNER JOIN cancellation ON d.acctno = cancellation.acctno
   --INNER JOIN acct ON d.acctno = acct.acctno
   --WHERE acct.currstatus = 'S'
 
 IF @excludenoitems = 0 
 BEGIN
 
 SELECT TOP 500 proposal.custid AS [Customer ID],
               proposal.acctno AS [Account Number],
               customer.title AS title,customer.firstname AS [First Name],
               customer.name AS [Last Name],acct.accttype AS [Type],
               agreement.empeenosale AS [Salesperson No],
               u.fullname AS [Salesperson Name],
               proposal.dateprop AS [Date Proposal],
               proposal.datechange AS  [Date Changed],
               --Isnull(code.codedescript, '') AS [Description],
               case when cl.acctno is not null and cl.loanstatus = 'R' then cl.ReferralReasons		--IP - 24/02/12 - #9598 - UAT 87
					else Isnull(code.codedescript, '') END AS [Description],
             --  CASE WHEN currstatus IN ( '0', 'U' ) THEN 'Unpaid'
													--ELSE 'Paid' END as 'Payment',	
			   case when currstatus IN ( '0', 'U' ) OR cl.acctno is not null THEN 'Unpaid'		--IP - 24/02/12 - #9598 - UAT 87
													ELSE 'Paid' END as 'Payment',				--IP - 24/02/12 - #9598 - UAT 87					
			    acct.dateacctopen /* CR 484 phase 2 */
FROM   delauthorise
INNER JOIN acct ON acct.acctno = delauthorise.acctno
INNER JOIN proposal ON delauthorise.acctno = proposal.acctno
INNER JOIN customer ON customer.custid = proposal.custid
INNER JOIN agreement ON agreement.acctno = proposal.acctno
INNER JOIN Admin.[User] u ON u.Id = agreement.empeenosale
LEFT OUTER JOIN CashLoan CL ON acct.acctno = CL.AcctNo
LEFT OUTER JOIN code ON code.code = proposal.reason AND code.category LIKE 'SN1'				--IP - 10/01/12 - #3780 - Re-instated LEFT JOIN
WHERE  ( @acctno = '000000000000' OR delauthorise.acctno = @acctno )
       --AND delauthorise.acctno LIKE @branchrestriction
       AND (delauthorise.acctno LIKE @branchrestriction or delauthorise.acctno = @acctno) --IP - 05/12/11 - #8777 - LW73607
       AND acct.currstatus != 'S'
       AND proposal.propresult != 'X' --exclude rejected accounts - JJ           
       AND ( @refcode IS NULL OR (proposal.reason = @refcode  and @PendingCL=0 and @cashLoanQual=0))			-- #9598 jec
       AND EXISTS (SELECT 1
                   FROM   proposalflag p
                   INNER JOIN code c ON p.checktype = c.code
					 WHERE  ( @holdflags IS NULL
                            OR (@Singleflagonly = 0 AND (p.checktype LIKE @holdflags and @cashLoanQual = 0 or (p.checktype ='R' and Customer.LoanQualified=1  and @cashLoanQual = 1 )))
                            OR (@Singleflagonly = 1 AND exists(select * from proposalflag p2 where p.custid = p2.custid and ((p2.checktype = @holdflags and @cashLoanQual = 0) or( p.checktype ='R' and Customer.LoanQualified=1  and @cashLoanQual = 1 )) -- #9598 jec 27/02/12
															AND not exists(select * from proposalflag p3 where p2.custid = p3.custid and p3.checktype != @holdflags and p3.datecleared is null)  )		-- #9598 jec 27/02/12
													) 
                          )
                                
                   AND c.category = 'PH1'
                   AND p.Acctno = acct.acctno AND p.custid = proposal.custid 
                   AND p.datecleared IS NULL
                   AND p.dateprop = proposal.dateprop)
       AND ( @excludeUnpaid = 0
              OR acct.currstatus NOT IN ( '0', 'U' ) )
       AND (@pendingCL = 0 OR (ISNULL(CL.LoanStatus,'') = 'R' AND CL.AcctNo IS NOT NULL))				-- #9598 jec 27/02/12
END
ELSE
BEGIN

SELECT  TOP 500 proposal.custid AS [Customer ID],
               proposal.acctno AS [Account Number],
               customer.title AS title,customer.firstname AS [First Name],
               customer.name AS [Last Name],acct.accttype AS [Type],
               agreement.empeenosale AS [Salesperson No],
               u.fullname AS [Salesperson Name],
               proposal.dateprop AS [Date Proposal],
               proposal.datechange AS  [Date Changed],
               Isnull(code.codedescript, '') AS [Description],
               CASE WHEN currstatus IN ( '0', 'U' ) THEN 'Unpaid'
													ELSE 'Paid' END,acct.dateacctopen /* CR 484 phase 2 */
FROM   delauthorise
INNER JOIN acct ON acct.acctno = delauthorise.acctno
INNER JOIN proposal ON delauthorise.acctno = proposal.acctno
INNER JOIN customer ON customer.custid = proposal.custid
INNER JOIN agreement ON agreement.acctno = proposal.acctno
INNER JOIN Admin.[User] u ON u.Id = agreement.empeenosale
LEFT OUTER JOIN CashLoan CL ON acct.acctno = CL.AcctNo
LEFT OUTER JOIN code ON code.code = proposal.reason AND code.category LIKE 'SN1'
WHERE  ( @acctno = '000000000000' OR proposal.acctno = @acctno )
       --AND delauthorise.acctno LIKE @branchrestriction
       AND (delauthorise.acctno LIKE @branchrestriction or delauthorise.acctno = @acctno)	--IP - 05/12/11 - #8777 - LW73607
       AND acct.currstatus != 'S'
       AND proposal.propresult != 'X' --exclude rejected accounts - JJ    
       AND ( @refcode IS NULL OR (proposal.reason = @refcode  and @PendingCL=0 and @cashLoanQual=0))			-- #9598 jec
       AND EXISTS (SELECT 1
                   FROM   proposalflag p
                   INNER JOIN code c ON p.checktype = c.code
                    WHERE  ( @holdflags IS NULL
                            OR (@Singleflagonly = 0 AND (p.checktype LIKE @holdflags and @cashLoanQual = 0 or (p.checktype ='R' and Customer.LoanQualified=1  and @cashLoanQual = 1 )))
                            OR (@Singleflagonly = 1 AND exists(select * from proposalflag p2 where p.custid = p2.custid and ((p2.checktype = @holdflags and @cashLoanQual = 0) or( p.checktype ='R' and Customer.LoanQualified=1  and @cashLoanQual = 1 )) --IP - 24/10/11 - #3896 - CR1232
													AND not exists(select * from proposalflag p3 where p2.custid = p3.custid and p3.checktype != @holdflags and p3.datecleared is null)		)
													) 
                          )
                   AND c.category = 'PH1'
                   AND p.Acctno = acct.acctno AND p.custid = proposal.custid 
                   AND p.datecleared IS NULL
                   AND p.dateprop = proposal.dateprop)
       AND ( @excludeUnpaid = 0
              OR acct.currstatus NOT IN ( '0', 'U' ) )
       AND EXISTS (SELECT 1 
				   FROM lineitem l
				   WHERE l.acctno = delauthorise.acctno
				   AND l.quantity > 0
				   AND l.itemtype = 'S')
AND (@pendingCL = 0 OR (ISNULL(CL.LoanStatus,'') = 'R' AND CL.AcctNo IS NOT NULL))		-- #9598 jec 27/02/12
END


GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End