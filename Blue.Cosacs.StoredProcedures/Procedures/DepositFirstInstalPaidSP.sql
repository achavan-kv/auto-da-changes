SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DepositFirstInstalPaidSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DepositFirstInstalPaidSP
END
GO

CREATE PROCEDURE DepositFirstInstalPaidSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DepositFirstInstalPaidSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check Deposit/First Instalment is paid
-- Author       : John Croft
-- Date         : 26 July 2007
--
-- This procedure will check whether a Credit Account that has been granted Instant Credit has paid either the deposit
-- or First Instalment depending on the allocated termstype. 
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 23/12/08  jec 70618 Check Doc Confirmation is cleared
-- ================================================
	-- Add the parameters for the stored procedure here

	 @piAccountNo		varchar(12),
	 @piPaidTot			decimal(12,2),
	 @empeeno			int,
	 @poInstDepPaid		char(1) out,
	 @return     int OUTPUT
AS

declare @outstbal decimal(12,2),		
		@deposit decimal(12,2),		
		@instalment decimal(12,2),		
		@InstantCredit char(1),
		@termstype varchar(2),
		@depositPaid char(1),
		@instalpredel char(1),
		@datefirst datetime, 	-- jec 23/12/08
		@arrears decimal(12,2), --CR1225 check new arrears flag
		@maxarrears decimal  ,		--CR1225 check new arrears flag
		@DCcleared datetime		--CR1225 check DC cleared

BEGIN
	set @poInstDepPaid='N'
	set @return=0

	if(@piPaidTot = 0)
	BEGIN

	declare @cheqdays int  
   
	 select @cheqdays = cheqdays  
	 from country  
  
  

		set @piPaidTot = (
							select isnull(sum(-transvalue), 0)
							from fintrans
							where acctno = @piAccountNo
								and transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX')
								AND datetrans + @cheqdays > getdate()  
								AND (paymethod%10) = 2   
						 )  
    
  set @piPaidTot = @piPaidTot +  
						  (  
							   select isnull(sum(-transvalue), 0)  
							   from fintrans  
							   where acctno = @piAccountNo  
								and transtypecode in ('PAY','COR','REF','RET','XFR','DDE','DDN','DDR','SCX')  
								AND (paymethod%10) != 2   
						)

	END

	select	--@outstbal = a.outstbal,
			@deposit = ag.deposit,
			@instalment = i.instalamount,
			@InstantCredit = i.InstantCredit,
			@datefirst = datefirst,
			@termstype= a.termstype,
			@depositPaid=t.depositPaid,
			@instalpredel=t.instalpredel

	From agreement ag inner join instalplan i on ag.acctno=i.acctno 
						inner join acct a on i.acctno=a.acctno inner join termstype t on a.termstype=t.termstype
	where ag.acctno=@piAccountNo
			and ag.agrmtno=1
			
	select @arrears = MAX(isnull(a.arrears/i.instalamount, 0))
	from acct a
	inner join instalplan i
		on i.acctno = a.acctno
	where i.instalamount > 0
		and a.acctno in (
						select acctno 
						from custacct 
						where custid = (
											select custid
											from custacct
											where acctno = @piAccountNo
												and hldorjnt = 'H'
										)
					)
 									
 	select @maxarrears = 
 	isnull(CONVERT(DECIMAL,value), 0)
	from CountryMaintenance
	where CodeName = 'IC_MaxArrearsLevel'
 
 	if @InstantCredit='Y'
		and (((abs(@piPaidTot)>=@deposit))		-- deposit paid
		and ((abs(@piPaidTot)-@deposit>=@instalment and @instalpredel='Y') or @instalpredel='N'))		-- first instalment paid
		and (@arrears <= @maxarrears)	-- arrears paid
		and @datefirst='1900-01-01'								-- no delivery 

		set @poInstDepPaid='Y'
		
	-- check if Doc confirmation is cleared	
	if  @InstantCredit='Y'
	Begin
		select @DCcleared=datecleared from dbo.proposalflag pf
			where pf.acctno=@piAccountNo and checktype='DC'
		If @DCcleared is null
			set @poInstDepPaid='N'		
	End
	
	--clear deposit flag

	if(abs(@piPaidTot)>=@deposit)
		update instantcreditflag
		set datecleared = getdate(),
			empeenopflg = @empeeno
		where  checktype = 'DEP'
			and acctno = @piAccountNo

	--clear first instalment flag

	if((abs(@piPaidTot)-@deposit>=@instalment and @instalpredel='Y') or @instalpredel='N')	-- first instalment paid
		update instantcreditflag
		set datecleared = getdate(),
			empeenopflg = @empeeno
		where  checktype = 'INST'
			and acctno = @piAccountNo

	-- clear instant credit arrears flag for all of customers accounts
	if @arrears <= @maxarrears
		update instantcreditflag
		set datecleared = getdate(),
			empeenopflg = @empeeno
		where  checktype = 'ARR'
			and acctno in (
							select acctno 
							from custacct 
							where custid = (
												select custid
												from custacct
												where acctno = @piAccountNo
													and hldorjnt = 'H'
											)
							)


End

SET @Return = @@ERROR

GO
GRANT EXECUTE ON DepositFirstInstalPaidSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 

