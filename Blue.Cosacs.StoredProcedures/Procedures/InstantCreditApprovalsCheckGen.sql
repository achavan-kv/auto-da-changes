

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[InstantCreditApprovalsCheckGen]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[InstantCreditApprovalsCheckGen]
GO

-- ===============================================================================================
-- Version:		<011> 
-- ===============================================================================================
CREATE PROCEDURE [dbo].[InstantCreditApprovalsCheckGen]

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : InstantCreditApprovalsCheckGen.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Generate Instant Credit Approvals/Cash Loan Qualification
-- Author       : John Croft
-- Date         : 26 July 2007
--
-- This procedure will process Customers AND their associated accounts to determine whether Instant Credit 
-- OR Cash Loans is available. The process parameter will be either "I" OR "L".
-- The procedure will either be executed as an EOD job, in which case all Customers will be processed
-- AND the Customer table will be flagged InstantCredit OR the procedure will be executed FROM the 
-- New/Revised account screen, in which case only that Customer will be processed AND a Result returned.
-- 
-- If the customer Id parameter is passed in as "??" then the current instalplan.instantcredit value will be returned 
-- AND no other processing will occur.
-- 
-- For Cash Loans the procedure will also be executed when loading the Basic Customer Details screen.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/09/07  jec CR906 Cash Loans
-- 13/12/07  jec 69428 Flag Instalplan.InstantCredit as R if refused IC so that account is only granted OR refused once.
-- 14/12/07  jec 69427 Exclude cancelled accounts
-- 14/05/09  IP CR983 Changes re Instant Credit report
-- 24/08/10 jec UAT6 - Origbr column renamed to Runno in Letter table
-- 10/09/10 jec UAT8 (5.1.9.0) - Cash Loan letter  (Removed 16/09/10)
-- 22/02/11  IP Sprint 5.10 - #3117 - Date Columns on #InstantCredit were not being updated.
-- 23/02/11	 IP Sprint 5.10 - #3195 - Joint Customers were not being selected.
-- 24/02/11  IP #3196 - LW73237 - Exclude Cancelled accounts
-- 28/02/11  IP Sprint 5.11 - #3237 - Previously incorrect Country Parameter was used to check employment date changed. Changed to use DATEADD FROM DATEDIFF
--			 for better accuracy when checking dates.
-- 01/03/11  IP Sprint 5.11 - #3247 - Changed to use DATEADD FROM DATEDIFF for better accuracy when checking dates (customers address changed)
-- 04/03/11  IP Sprint 5.11 - #3194 - Only if referral months country parameter > 0 then check if the proposal for the account OR for customers were 
--			 referred within the parameter months, otherwise do not check for referral.
-- 07/03/11 IP - Only check @SettAcctLen for Instant Credit
-- 29/07/11 jec CR1212 RI changes
-- 19/09/11 jec CR1232 Cash Loan changes
-- 13/10/11 ip  #8439 - CR1232 - Staff who do not have an available spend should not qualify
-- 18/10/11 jec #8463 not all staff accounts are qualifying for loan
-- 20/10/11 jec #8478 - CR1232 Accounts to check - HP, RF OR Both
-- 21/10/11 jec #3900 - CR1232 Referral messages
-- 24/10/11 jec #3895 - CR1232 Refered accounts
-- 26/10/11 ip  #3904 - CR1232 Cash Loan letter - New Customers
-- 26/10/11 ip  #3905 - CR1232 Cash Loan letter - Previous Customers that have a Cash loan settled x months ago
-- 27/10/11 jec #8532 Application error in Cash Loan screen
-- 27/10/11 ip  #3906 - CR1232 Cash Loan Letter - Current Customers that have paid a percentage of their Cash Loan
-- 28 10/11 jec #8476 LW74124 - Instant Credit Issue 
-- 31/10/11 jec #8509 Referral reason for Address AND Employment change 
--              #8546 Customer not qualified after passing the status check period
-- 02/11/11 jec #3895 CR1232 Referred accounts - check referral table
-- 04/11/11 jec #8586 return Cash Loan value used
-- 07/11/11 jec #8558 CR1232 cash loans referral for score/credit limit
-- 08/11/11 jec #8565 CR1232 Remove Letter validity period
-- 14/11/11 ip  #8622 CR1232 LoanS letter was incorrectly being generated for a customer that has a current Cash Loan account
-- 15/11/11 ip  #8624 CR1232 LoanS letter was incorrectly being generated. No need to check if current Cash Loan was opened after most recent settled.
-- 17/11/11 IP  #8645 CR1232 Use the sum of outstanding balance for Cash Loan accounts to determine amount available for Cash Loan to cater for payments
-- 22/11/11 jec #8596 ReSET unblock CashLoan in EOD run	
-- 29/11/11 jec #8756 Error on loading the custid in the cash loan screen
-- 07/12/11 jec Merge/Copy 5.13		
-- 06/01/12 ip  #9356 - Only update Staff = Y if Currstatus = 9
-- 01/10/12 jec #10354 LW75091 - Check Joint Date Residence change  - (match Holder code) 
-- 18/06/13 ip  #13949 - Cash Loan not qualified.
-- 19/06/13 ip  #13948 - Cash Loan Qualification
-- 07/01/14 ip  #16883 - Cash Loans not qualifying when loading in Cash Loan screen.
-- 05/03/14 jec #17649 - Subquery returned more than 1 value error
-- 25/03/14 ip  #17530 - Customers credit cannot be blocked in order to qualify for Cash Loan
-- 01/08/14 ip  #19353 - CR18831 - Restore Instant Credit Report
-- 06/03/15 ip  CR22543 - New Cash Loan changes
-- 23/07/20  Added index for #AcctsToCheck on acct column
-- 23/07/20  Avoid multipal call on instant credit approvl 
-- 25/07/20  Added temporary table for customer for updating instant credit as suggested by Javier Parada
-- 26/07/20  Updated the customer value in the temporary table first and at the end updated the actual customer table
-- 29/07/20  Added condition before updating the Customer table and compared if any changed value foudn agains the customer then only update it.
-- 29/07/20  Added time slot condition to check customer id is blank then return with Loanqualified and instant redit as "x"
-- 24/08/20  As per log 7199902 P2 sp takes 2+ hour for execution. So that after an hour CashLoan Qualification EOD shows time out error. issue is fixed.
-- 25/08/20 Modified the index added by the developer (Included Propdate and acctno in the index)
-- ================================================
	-- Add the parameters for the stored procedure here

		@piCustomerID		VARCHAR(20)=' ',
		@piAccountNo		VARCHAR(12)='',
		@piProcess			CHAR(1)=' ',	-- I=Instant Credit, L=Cash Loan	-- CR906 
		@poInstantCredit	CHAR(1) OUT,
		@poLoanQualified	CHAR(1) OUT,			-- CR906
		@return     INT OUTPUT
AS
BEGIN
	-------------------------------------------------------------------------------------------
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--SET @piCustomerID ='SOB120282'
	-- if Customer does not exist - return  parameters as X
	SET @return=0

	-- Check if Customer id is blank between morning 6 to Evening 6 then return Loanqualified and instant redit as "x"
	DECLARE @time AS TIME
	SET @time =  CONVERT(VARCHAR(32),GETDATE(),108)
	
	IF @time > '07:00:00' AND @Time <'19:00:00'       --- used 24 hrs format
	BEGIN
		IF @piCustomerID=''
		BEGIN
			SET @poLoanQualified='X'
			SET @poInstantCredit='X'
			RETURN
		END
	END
	
	-- Create Temporary Customer Table
	CREATE TABLE #CustomerForCreditUpdate
	(
		custid VARCHAR(20),
		InstantCredit CHAR(1) NULL,
		loanqualified BIT,
		CashLoanBlocked CHAR(1) NULL,
		CashLoanNew BIT,
		CashLoanRecent BIT,
		CashLoanExisting BIT,
		CashLoanStaff BIT,
		RFCreditLimit MONEY,
		AvailableSpend MONEY,
		creditblocked TINYINT
	)

	CREATE INDEX ix_CustomerForCreditUpdate_custid ON #CustomerForCreditUpdate(custid)	

	---Avoid multipal call on instant credit approvl 
	IF ( EXISTS (SELECT TOP 1 piCustomerID 
				FROM [dbo].[InstantCreditApprovalsCheckGen_Val] 
				WHERE (piCustomerID = @piCustomerID ) 
				AND DATEDIFF(SECOND,[ApprovlDate],GETDATE()) > 300 
				ORDER BY [ApprovlDate] DESC) 
				OR (SELECT COUNT(piCustomerID) 
					FROM [dbo].[InstantCreditApprovalsCheckGen_Val] 
					WHERE (piCustomerID = @piCustomerID) ) = 0)
	BEGIN
		DECLARE @datequalification DATETIME 
		SET @datequalification = GETDATE()

		IF @piCustomerID!=' ' AND @piCustomerID!='??' AND NOT EXISTS(SELECT TOP 1 'A' FROM customer WHERE CustId=@piCustomerID)
		BEGIN
			SET @poLoanQualified='X'
			SET @poInstantCredit='X'
			RETURN
		END
	-- Create Temporary customer table to avoid blocking in the system -- 26-07-2020
		IF @piCustomerID!=' ' AND @piCustomerID!='??'
		BEGIN
			INSERT INTO #CustomerForCreditUpdate
			(
			custid,
			InstantCredit,
			loanqualified,
			CashLoanBlocked,
			CashLoanNew,
			CashLoanRecent,
			CashLoanExisting,
			CashLoanStaff,
			RFCreditLimit,
			AvailableSpend,
			creditblocked )
			SELECT custid,
			InstantCredit,
			loanqualified,
			CashLoanBlocked,
			CashLoanNew,
			CashLoanRecent,
			CashLoanExisting,
			CashLoanStaff,
			RFCreditLimit,
			AvailableSpend,
			creditblocked 
			FROM customer 
			WHERE CustId=@piCustomerID

		END
		 
		--DECLARE @monthsqualifyloan int ,@maxletterdate DATETIME, @keepexistingloan TINYINT				-- #8565
		--SELECT @monthsqualifyloan = convert(INT,value) FROM CountryMaintenance WHERE codename = 'CL_CashLoanLetter'
		--SET @keepexistingloan = 0 
		--IF @piAccountNo != ''
		--BEGIN
		--	SELECT @maxletterdate= ISNULL(dateacctlttr,'1-JAN-1900') FROM letter WHERE lettercode = 'LOAN' AND acctno =@piAccountNo 	
		--	IF @maxletterdate != '1-Jan-1900'
		--	BEGIN
		--		IF DATEADD(MONTH,@monthsqualifyloan,@maxletterdate) > GETDATE()
		--		BEGIN
		--			IF EXISTS (SELECT * FROM customer WHERE custid = @picustomerid 
		--			AND loanqualified =1)
		--				SET @keepexistingloan = 1
										
		--		END
										
		--	END
		--END
							   
		-- start debugging statements 

		--	declare @piCustomerID		VARCHAR(20),		--debugging only
		--			@poInstantCredit	CHAR(1),			--debugging only
		--			@piProcess			CHAR(1),			--debugging only
		--			@piAccountNo		VARCHAR(12),		--debugging only
		--			@poLoanQualified	CHAR(1),			--debugging only  
		--			@return     int 						--debugging only
		--	SET @piCustomerID=' '					--debugging only		'AW170357'
		--	SET @piAccountNo=' '					--debugging only		'AW170357'
		--	SET @piProcess='I'					--debugging only	 I=Instant Credit, L=Cash Loan	
		--	drop table #InstantCredit		-- testing only
		--	drop table #instantcredit_save		-- testing only
		--	drop table #JointAccounts		-- testing only
		--	drop table #Settled				-- testing only
		----  drop table #settledAll			-- testing only
		--	drop table #Delivered			-- testing only
		--	drop table #SpouseAccounts		-- testing only
		--	drop table #ArrearsLevel		-- testing only
		--	drop table #DateAuth			-- testing only
		--	drop table #CashLoan			-- testing only
		--  drop table #RefMessages			-- testing only
		--  drop table #AcctsToCheck			-- testing only
		--  drop table #Refstatus		-- testing only

		-- end debugging statements 
		-------------------------------------------------------------------------
		-- Get the correct parameter for the fuction.

		DECLARE @P_EmployChgManApprove VARCHAR(30),
		@P_ExistAccountLength VARCHAR(30),
		@P_HighSettStat2Yr VARCHAR(30),
		@P_HighStatusTimeFrame VARCHAR(30),
		@P_LoanMinCredScore VARCHAR(30),        
		@P_LoanRFpcAvail VARCHAR(30),
		@P_MaxAgrmtTotal VARCHAR(30),
		@P_MaxArrearsLevel VARCHAR(30),
		@P_MaxLoanAmount VARCHAR(30),
		@P_ResidenceChgManApprove VARCHAR(30),
		@P_SettledCredMonths VARCHAR(30),
		@P_MinCredScore VARCHAR(30),
		@P_addresscheckscore VARCHAR(30), --CR1225
		@P_employmentcheckscore VARCHAR(30), --CR1225
		@P_ReferralMonths VARCHAR(30), --CR1225
		@P_ReviseMonths VARCHAR(30), --CR1225
		@P_JointQualification VARCHAR(30), --CR1225
		@P_addresscheckMonths VARCHAR(30), --CR1225
		@P_employmentcheckMonths VARCHAR(30), --CR1225
		@P_SettAccountLength VARCHAR(30), --CR1225
		@P_code VARCHAR(5),
		@P_AccTypeToQualify VARCHAR(30),	-- CR1232
		@P_MinLoanAmount VARCHAR(30),		-- CR1232
		@P_MaxPctRFavail VARCHAR(30),	-- CR1232
		@P_ReferralArrears VARCHAR(30),	 -- #3900
		@P_ReferralEmployment VARCHAR(30),	-- #3900
		@P_ReferralPercentage VARCHAR(30),	-- #3900
		@P_ReferralRescored VARCHAR(30),	-- #3900
		@P_ReferralResidence VARCHAR(30),	-- #3900
		@P_ReferralStatus VARCHAR(30),	-- #3900
		@P_CashLoanLetterPrevSettMths VARCHAR(30), --CR1232
		@P_CashLoanPercentagePaid VARCHAR(30),	--CR1232	
		@P_RecentCustAccountLength VARCHAR(30),
		@P_HighSettStatRecentCust VARCHAR(30),
		@P_RecentCustMaxArrearsLevel VARCHAR(30),
		@P_SettledMinTermLength VARCHAR(30),
		@P_LoanMinCredScoreRecent VARCHAR(30),
		@P_LoanMinCredScoreNew VARCHAR(30)

		IF @piProcess = 'I'
		BEGIN
			SET @P_code = 'IC_'
		END

		IF @piProcess = 'L'
		BEGIN
			SET @P_code = 'CL_'
		END

		SELECT  
		@P_EmployChgManApprove = @P_code + 'EmployChgManApprove',
		@P_ExistAccountLength = @P_code + 'ExistAccountLength',
		@P_HighSettStat2Yr = @P_code + 'HighSettStat2Yr',
		@P_HighStatusTimeFrame = @P_code + 'HighStatusTimeFrame',
		@P_LoanMinCredScore = @P_code + 'LoanMinCredScore',
		@P_LoanRFpcAvail = @P_code + 'LoanRFpcAvail',
		@P_MaxAgrmtTotal = @P_code + 'MaxAgrmtTotal',
		@P_MaxArrearsLevel = @P_code + 'MaxArrearsLevel',
		@P_MaxLoanAmount = @P_code + 'MaxLoanAmount ',
		@P_ResidenceChgManApprove = @P_code + 'ResidenceChgManApprove',
		@P_SettledCredMonths = @P_code + 'SettledCredMonths',
		@P_MinCredScore = @P_code + 'MinCredScore',
		--CR1225 new parameters
		@P_addresscheckscore = @P_code + 'AddressCheckScore',
		@P_employmentcheckscore = @P_code + 'EmploymentCheckScore',
		@P_ReferralMonths = @P_code + 'ReferralMonths',
		@P_ReviseMonths = @P_code + 'ReviseMonths',
		@P_JointQualification = @P_code + 'JointQualification',
		@P_addresscheckMonths = @P_code + 'AddressMonths',
		@P_employmentcheckMonths = @P_code + 'EmployMonths',
		@P_SettAccountLength= @P_code + 'settledmonths',
		@P_AccTypeToQualify= 'CL_AccountType',		-- CR1232
		@P_MinLoanAmount= 'CL_MinLoanAmount',		-- CR1232
		@P_MaxPctRFavail= 'CL_MaxPctRFavail',			-- CR1232
		@P_ReferralArrears= 'CL_ReferralArrears',			-- #3900
		@P_ReferralEmployment= 'CL_ReferralEmployment',			-- #3900
		@P_ReferralPercentage= 'CL_ReferralPercentage',			-- #3900
		@P_ReferralRescored= 'CL_ReferralRescored',			-- #3900
		@P_ReferralResidence= 'CL_ReferralResidence',			-- #3900
		@P_ReferralStatus= 'CL_ReferralStatus',			-- #3900
		@P_CashLoanLetterPrevSettMths = 'CL_CashLoanLetterPrevSettMths',	--CR1232
		@P_CashLoanPercentagePaid = 'CL_PercentagePaid',	--CR1232,
		@P_RecentCustAccountLength = @P_code + 'RecentCustAccountLength',
		@P_HighSettStatRecentCust = @P_code + 'HighSettStatRecentCust',
		@P_RecentCustMaxArrearsLevel = @P_code + 'RecentCustMaxArrearsLevel',
		@P_SettledMinTermLength = @P_code + 'SettledMinTermLength',
		@P_LoanMinCredScoreRecent = @P_code + 'LoanMinCredScoreRecent',
		@P_LoanMinCredScoreNew = @P_code + 'LoanMinCredScoreNew'

		SET @return=0	
		SET @poInstantCredit='N'
		--IF @keepexistingloan =0				-- #8565
		SET @poLoanQualified='N'
		--ELSE 
		--	SET @poLoanQualified = 'Y'

		-- if customerid passed in = ?? to then return instalplan.instantcredit only
		IF @piCustomerID='??'		
		BEGIN	
			SET @poInstantCredit=(SELECT instantCredit FROM Instalplan WHERE acctno=@piAccountNo)
		END
		ELSE
		BEGIN		-- Start of Checking
			DECLARE	@MinCredScore SMALLINT,
			@SettCredMonths		SMALLINT,
			--@SettCashMonths		SMALLINT,
			@ExistAcctLen		SMALLINT,
			@MaxAgrmtTotal		INT,
			@EmployChangeApp	VARCHAR(5),
			@ResidenceChangeApp	VARCHAR(5),
			@MaxArrearsLevel	DECIMAL(3,1),
			@maxstatus		CHAR(1),
			@ExistInstCredit	CHAR(1),
			@ExistLoanQualified	BIT,			-- CR906
			@LoanMinCredScore	SMALLINT,		-- CR906
			@LoanRFpcAvail		INT,		-- CR906 --RM - 06.05.2010 CR1082 has error if percentage too large to fit in small int
			@MaxLoanAmount		INT,			-- CR906
			@PreapprovalDate	SMALLDATETIME,		-- CR983 --IP - 10/03/09
			@HighStatusTimeFrame INT,
			--CR1225 parameters
			@referralmonths INT,
			@revisemonths INT,
			@addresscheckscore INT,
			@employmentcheckscore INT,
			@addresscheckmonths INT,
			@employmentcheckmonths INT,
			@jointqualification VARCHAR(5),
			@SettAcctLen		SMALLINT,
			@MinLoanAmount		INT,			-- CR1232
			@MaxPctRFavail		INT,			-- CR1232
			@AccTypeToQualify	VARCHAR(5),		-- CR1232
			@AcctsToCheck		BIT,			-- #8478
			@ReferralEmployment		BIT,			-- #3900
			@ReferralPercentage		BIT,			-- #3900
			@ReferralRescored		BIT,			-- #3900
			@ReferralResidence		BIT,			-- #3900
			@ReferralStatus		BIT,			-- #3900
			@ReferralArrears		BIT,			-- #3900
			@AddressChange		INT,				-- CR1232	
			@CashLoanLetterPrevSettMths INT,		--CR1232
			@CashLoanPercentagePaid DECIMAL,		--CR1232
			@RecentCustAcctLen		SMALLINT,
			@MaxstatusRecentCust    CHAR(1),
			@RecentCustMaxArrearsLevel	DECIMAL(3,1),
			@SettledMinTermLength INT,
			@MinCredScoreRecent SMALLINT,
			@MinCredScoreNew SMALLINT

			SET @ExistInstCredit='N'
			SET @ExistLoanQualified='0'		-- CR906
			SET @AcctsToCheck=0			-- #8478
			-- if customerid passed in check existing Instant Credit flag
			IF @piCustomerID!=' ' AND @piProcess = 'I'		-- CR906		
			BEGIN	
				SET @revisemonths= (SELECT value FROM countrymaintenance WHERE codename=@P_ReviseMonths)		-- CR1225
				SET @ExistInstCredit=(SELECT ISNULL(instantCredit,'N') 
				FROM Instalplan i
				INNER JOIN acct a ON a.acctno = i.acctno
				WHERE i.acctno=@piAccountNo
				AND DATEDIFF(MONTH, a.dateacctopen, GETDATE())<= @revisemonths)
			END
			-- if customerid passed in check existing Loan Qualified flag
			IF @piCustomerID!=' ' AND @piProcess = 'L'		-- CR906		
			BEGIN	
				SET @ExistLoanQualified=(SELECT ISNULL(LoanQualified,'0') 
				FROM #CustomerForCreditUpdate
				WHERE custid=@piCustomerID)
			END

			IF (SELECT currstatus FROM acct WHERE acctno = @piAccountNo) = '9' 
						AND @piProcess = 'L' AND @piAccountNo != ''
			BEGIN
				SET @poLoanQualified = 'Y'
				RETURN 
			END

			-- if Exisisting Instant Credit is not 'Y' execute full process (this applies to single customer OR EOD mode)
			IF ISNULL(@ExistInstCredit,'N')='N'	-- jec 13/12/07 69428  !='Y'
			BEGIN
				-- Get Instant Credit Country parameters
				SET @MinCredScore= (SELECT value FROM countrymaintenance WHERE codename=@P_MinCredScore)
				SET @SettCredMonths= (SELECT value FROM countrymaintenance WHERE codename=@P_SettledCredMonths)
				--SET @SettCashMonths= (SELECT value FROM countrymaintenance WHERE codename='SettledCashMonths')
				SET @ExistAcctLen= (SELECT value FROM countrymaintenance WHERE codename=@P_ExistAccountLength)
				SET @MaxAgrmtTotal= (SELECT value FROM countrymaintenance WHERE codename=@P_MaxAgrmtTotal)
				SET @EmployChangeApp= (SELECT value FROM countrymaintenance WHERE codename=@P_EmployChgManApprove)
				SET @ResidenceChangeApp= (SELECT value FROM countrymaintenance WHERE codename=@P_ResidenceChgManApprove)
				SET @MaxArrearsLevel= (SELECT value FROM countrymaintenance WHERE codename=@P_MaxArrearsLevel)
				SET @maxstatus= (SELECT value FROM countrymaintenance WHERE codename=@P_HighSettStat2Yr)	
				SET @HighStatusTimeFrame = (SELECT value FROM countrymaintenance WHERE codename=@P_HighStatusTimeFrame)
				SET @SettAcctLen= (SELECT value FROM countrymaintenance WHERE codename=@P_SettAccountLength) --CR1225
								
				IF @piProcess = 'I'			-- Instant credit only
				BEGIN
					SET @referralmonths= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralMonths)		-- CR1225
					SET @addresscheckscore= (SELECT value FROM countrymaintenance WHERE codename=@P_addresscheckscore)			-- CR1225
					SET @employmentcheckscore= (SELECT value FROM countrymaintenance WHERE codename=@P_employmentcheckscore)			-- CR1225
					SET @jointqualification = (SELECT value FROM countrymaintenance WHERE codename=@P_jointqualification) --CR1225
					SET @addresscheckmonths= (SELECT value FROM countrymaintenance WHERE codename=@P_addresscheckmonths)			-- CR1225
					SET @employmentcheckmonths= (SELECT value FROM countrymaintenance WHERE codename=@P_employmentcheckmonths)			-- CR1225	
				END
								
				--CR983 - IP - 10/03/09
				SET @PreapprovalDate = (SELECT GETDATE())

				--CR983 - SELECT the Country Parameter values into a permanent table as a record of what the values
				--were at the time of running
				INSERT INTO InstantCreditParameters
				( 
					MinCredScore ,SettCredMonths ,
					ExistAcctLen, MaxAgrmtTotal,
					EmployChangeApp ,ResidenceChangeApp ,
					MaxArrearsLevel ,Maxstatus , PreapprovalDate, 
					ReferralMonths, AddressCheckScore, EmploymentcheckScore, --cr1225
					jointqual, AddressMonths, EmployMonths, settledlength --CR1225
				)
				SELECT @MinCredScore, @SettCredMonths, --@SettCashMonths, 
				@ExistAcctLen, @MaxAgrmtTotal,
				@EmployChangeApp, @ResidenceChangeApp, 
				@MaxArrearsLevel, @maxstatus, @PreapprovalDate, 
				@ReferralMonths, @AddressCheckScore, @EmploymentcheckScore, --CR1225
				@jointqualification, @AddressCheckMonths, @EmploymentcheckMonths, @SettAcctLen --CR1225
								
				IF @piProcess = 'L'			-- Cash Loan processing
				BEGIN
					SET @MinCredScore= (SELECT value FROM countrymaintenance WHERE codename=@P_LoanMinCredScore)		-- CR906
					SET @LoanRFpcAvail= (SELECT value FROM countrymaintenance WHERE codename=@P_LoanRFpcAvail)			-- CR906
					SET @MaxLoanAmount= (SELECT value FROM countrymaintenance WHERE codename=@P_MaxLoanAmount)			-- CR906
					SET @MinLoanAmount= (SELECT value FROM countrymaintenance WHERE codename=@P_MinLoanAmount)			-- CR1232
					SET @MaxPctRFavail= (SELECT value FROM countrymaintenance WHERE codename=@P_MaxPctRFavail)			-- CR1232
					SET @AccTypeToQualify= (SELECT value FROM countrymaintenance WHERE codename=@P_AccTypeToQualify)	-- CR1232
					SET @addresscheckmonths= (SELECT value FROM countrymaintenance WHERE codename=@P_addresscheckmonths)			-- CR1232
					SET @employmentcheckmonths= (SELECT value FROM countrymaintenance WHERE codename=@P_employmentcheckMonths)			-- CR1232
					SET @referralmonths= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralMonths)		-- #3895
					SET @ReferralEmployment= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralEmployment)		-- #3900
					SET @ReferralPercentage= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralPercentage)		-- #3900
					SET @ReferralRescored= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralRescored)		-- #3900
					SET @ReferralResidence= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralResidence)		-- #3900
					SET @ReferralStatus= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralStatus)		-- #3900
					SET @ReferralArrears= (SELECT value FROM countrymaintenance WHERE codename=@P_ReferralArrears)		-- #3900
					SET @CashLoanLetterPrevSettMths = (SELECT value FROM countrymaintenance WHERE codename = @P_CashLoanLetterPrevSettMths)	-- CR1232
					SET @CashLoanPercentagePaid = (SELECT value FROM countrymaintenance WHERE codename = @P_CashLoanPercentagePaid)	-- CR1232
					SET @RecentCustAcctLen= (SELECT value FROM countrymaintenance WHERE codename=@P_RecentCustAccountLength)
					SET @MaxstatusRecentCust= (SELECT value FROM countrymaintenance WHERE codename=@P_HighSettStatRecentCust)	
					SET @RecentCustMaxArrearsLevel= (SELECT value FROM countrymaintenance WHERE codename=@P_RecentCustMaxArrearsLevel)
					SET @SettledMinTermLength= (SELECT value FROM countrymaintenance WHERE codename=@P_SettledMinTermLength)
					SET @MinCredScoreRecent = (SELECT value FROM CountryMaintenance WHERE codename = @P_LoanMinCredScoreRecent)
					SET @MinCredScoreNew = (SELECT value FROM countrymaintenance WHERE codename = @P_LoanMinCredScoreNew)
				END
				-- Create Tempory tables
				 	
				CREATE TABLE #InstantCredit
				(
				Hcustid				VARCHAR(20),
				Hscore				SMALLINT,
				Hdatechange			DATETIME,
				HdateSettCred		DATETIME,
				HdateSettcash 		DATETIME,
				HexistAccLen		SMALLINT,
				HhighSettStatus		CHAR(1),
				HspouseBDWlegal		CHAR(1),
				HReferred			CHAR(1),
				HmaxArrearsLevel	DECIMAL(9,2),
				HRFpcAvail			INT,			-- CR906  --RM - 06.05.2010 CR1082 has error if percentage too large to fit in small int
				HRFpcCashLoan		INT,			-- CR1232
				HAcctType			CHAR(4),		-- CR1232
				HCustAddDateChange	DATETIME,
				HEmploymentDateChange DATETIME,
				Jcustid				VARCHAR(20),
				Jscore				SMALLINT,
				Jdatechange			DATETIME,
				JdateSettCred		DATETIME,
				JdateSettcash		DATETIME,
				JexistAccLen		SMALLINT,
				JhighSettStatus		CHAR(1),
				JspouseBDWlegal		CHAR(1),
				JReferred			CHAR(1),
				JmaxArrearsLevel	DECIMAL(9,2),
				JRFpcAvail			INT,			-- CR906  --RM - 06.05.2010 CR1082 has error if percentage too large to fit in small int
				JRFpcCashLoan		INT,			-- CR1232
				JAcctType			CHAR(2),		-- CR1232
				JCustAddDateChange	DATETIME,
				JEmploymentDateChange DATETIME,
				InstantCredit		CHAR(1),
				LoanQualified		CHAR(1),			-- CR906
				PreapprovalDate		DATETIME,			-- CR983
				Staff				CHAR(1),			-- #8463			-- CR983
				HNewCustomer         BIT,
				HRecentCustomer      BIT,
				HExistingCustomer    BIT,
				JNewCustomer         BIT,
				JRecentCustomer      BIT,
				JExistingCustomer    bit
				)

				CREATE TABLE #JointAccounts
				(
				custid				VARCHAR(20),
				acctno				VARCHAR(12),
				dateacctopen		DATETIME,
				Hcustid				VARCHAR(20)
				)

				CREATE TABLE #settled
				(
				custid				VARCHAR(20),
				datesettled			DATETIME,
				status				CHAR(1)		
				)

				CREATE TABLE #Delivered
				(
				custid				VARCHAR(20),
				dateDelivered		DATETIME		
				)

				CREATE TABLE #SpouseAccounts
				(
				custid				VARCHAR(20),
				acctno				VARCHAR(12),
				hldorjnt			CHAR(1),
				Hcustid				VARCHAR(20),
				currstatus			CHAR(1),
				HighStatus			CHAR(1)
				)

				CREATE TABLE #ArrearsLevel
				(
				custid				VARCHAR(20),
				acctno				VARCHAR(12),
				hldorjnt			CHAR(1),
				ArrearsLevel		DECIMAL(9,2)		
				)

				CREATE TABLE #DateAuth			-- jec 03/10/07
				(
				custid				VARCHAR(20),
				acctno				VARCHAR(12),
				dateAuth			DATETIME			
				)
								
				CREATE TABLE #CashLoan			-- CR1232
				(
				custid				VARCHAR(20),
				OutstBalLoan		money			
				)
				-- Accounts to check - HP, RF OR both 
				CREATE TABLE #AcctsToCheck			-- CR1232 -- #8478
				(
				acctno				VARCHAR(12),
				acctType            CHAR(1)					
				)
				CREATE TABLE #RefStatus			-- CR1232 -- #3900
				(
				acctno				VARCHAR(12)			
				)
								
				CREATE TABLE #RefMessages			-- CR1232
				(
				Msg1				CHAR(1),
				Msg2				CHAR(1),
				Msg3				CHAR(1),
				Msg4				CHAR(1),
				Msg5				CHAR(1),
				Msg6				CHAR(1)					
				)

				CREATE TABLE #SettledDelivered
				(
				Custid				VARCHAR(20),
				DateDelivered		DATETIME,
				TermLength          INT,
				DateSettled         DATETIME		
				)

				CREATE TABLE #DeliveredBoth
				(
				Custid				VARCHAR(20),
				DateDelivered		DATETIME		
				)

				CREATE TABLE #TempProposal
			   	(
					custid VARCHAR(20) NULL,
					dateprop DATETIME NULL,
					acctno VARCHAR(12) NULL,
					points INT NULL,
					datechange DATETIME NULL,
					propresult char(1) NULL
			   	)			   		

			   	CREATE TABLE #maxDate
			   	(
					custid VARCHAR(20) NULL,
					dateprop DATETIME NULL,
					acctno VARCHAR(12) NULL
			   	)			   

				INSERT INTO #RefMessages
				SELECT 'N','N','N','N','N','N'			

				-- check available spend AND CREATE INDEXes if EOD mode
				IF @piCustomerID =' '
				BEGIN
					PRINT 'Calculating all available spend'
					EXEC dn_CustomerCalculateAvailableSpendAll
					CREATE INDEX ix_tem_InstantCredit_custid ON #InstantCredit(Hcustid)	
					CREATE INDEX ix_tem_JointAccounts_custid ON #JointAccounts(custid)	
					CREATE INDEX ix_tem_settled_custid ON #settled(custid)	
					CREATE INDEX ix_tem_Delivered_custid ON #Delivered(custid)	
					CREATE INDEX ix_tem_SpouseAccounts_custid ON #SpouseAccounts(custid)	
					CREATE INDEX ix_tem_ArrearsLevel_custid ON #ArrearsLevel(custid)	
				END

				-- Load Main Account Holders	
				INSERT INTO #InstantCredit (Hcustid,Hscore,Hdatechange,HdateSettCred,HdateSettcash,HexistAccLen,HhighSettStatus,HspouseBDWlegal,HReferred,HmaxArrearsLevel,HRFpcAvail, HCustAddDateChange, HEmploymentDateChange, -- CR906
				Jcustid,Jscore,Jdatechange,JdateSettCred,JdateSettcash,JExistAccLen,JHighSettStatus,JSpouseBDWlegal,JReferred,JmaxArrearsLevel,JRFpcAvail, JCustAddDateChange, JEmploymentDateChange,	-- CR906
				InstantCredit,LoanQualified, PreapprovalDate,Staff,HAcctType, -- #8463
				HNewCustomer, HRecentCustomer, HExistingCustomer,
				JNewCustomer, JRecentCustomer, JExistingCustomer)		
				SELECT DISTINCT custid,0,NULL,NULL,NULL,0,0,' ',' ',0,0, '1900-01-01', '1900-01-01',		-- CR906
				'NoJoint ',0,NULL,NULL,NULL,0,0,' ',' ',0,0, '1900-01-01', '1900-01-01',		-- CR906
				'N','N', @PreapprovalDate,'N',@AccTypeToQualify,		-- #8463
				0,0,0,
				0,0,0 
				FROM custacct
				WHERE hldorjnt='H' 
				AND (ISNULL(@piCustomerID,' ')=' '
				OR custid=@piCustomerID)

				-- Check if Customer id is blank (as part of EOD or some other process, add customer data to temp table
				IF @piCustomerID =' ' OR @piCustomerID =''
				BEGIN
					INSERT INTO #CustomerForCreditUpdate
					(
					custid,
					InstantCredit,
					loanqualified,
					CashLoanBlocked,
					CashLoanNew,
					CashLoanRecent,
					CashLoanExisting,
					CashLoanStaff,
					RFCreditLimit,
					AvailableSpend,
					creditblocked )
					SELECT custid,
					c.InstantCredit,
					c.loanqualified,
					CashLoanBlocked,
					CashLoanNew,
					CashLoanRecent,
					CashLoanExisting,
					CashLoanStaff,
					RFCreditLimit,
					AvailableSpend,
					creditblocked 
					FROM customer C
					INNER JOIN #InstantCredit I ON C.custid = I.Hcustid
				END 				

				IF ISNULL(@piCustomerID,' ')!=' '
				BEGIN
					-- Get Holder account WHERE this customer is a joint holder - Single Customer (@piCustomerID)
					INSERT INTO #InstantCredit (Hcustid,Hscore,Hdatechange,HdateSettCred,HdateSettcash,HexistAccLen,HhighSettStatus,HspouseBDWlegal,HReferred,HmaxArrearsLevel,HRFpcAvail,	-- CR906
					Jcustid,Jscore,Jdatechange,JdateSettCred,JdateSettcash,JExistAccLen,JHighSettStatus,JSpouseBDWlegal,JReferred,JmaxArrearsLevel,JRFpcAvail,	-- CR906
					InstantCredit,LoanQualified, PreapprovalDate,Staff,HAcctType,
					HNewCustomer, HRecentCustomer, HExistingCustomer,
					JNewCustomer, JRecentCustomer, JExistingCustomer)		-- #8463
					SELECT DISTINCT caJ.custid,0,NULL,NULL,NULL,0,0,' ',' ',0,0,	-- CR906
					'Joint ',0,NULL,NULL,NULL,0,0,' ',' ',0,0,			-- CR906
					'N','N',@PreapprovalDate,'N',@AccTypeToQualify,						-- #8463 
					0,0,0,
					0,0,0
					FROM custacct ca 
					INNER JOIN custacct caJ ON ca.acctno=caJ.acctno
					WHERE ca.hldorjnt='J' 
					AND (ISNULL(@piCustomerID,' ')=' ' OR ca.custid=@piCustomerID)
					AND caJ.hldorjnt='H'

					-- Get joint accounts - Single Customer (@piCustomerID)
					INSERT INTO #JointAccounts (custid,acctno,dateacctopen,Hcustid)
					-- #10354 - this code replaces above as not all joint CustomerId's were being returned
					SELECT caj.custid,caj.acctno,'1900-01-01','Holder' 
					FROM custacct ca 
					INNER JOIN custacct caj ON ca.acctno = caj.acctno
					WHERE ca.custid=@piCustomerID 
					AND caj.custid!=@piCustomerID 
					AND ca.hldorjnt='H'
					AND caj.hldorjnt = 'J' --#13949
				END
				ELSE
				BEGIN
					-- Get joint accounts - All Customers (EOD)
					INSERT INTO #JointAccounts (custid,acctno,dateacctopen,Hcustid)
					SELECT custid,acctno,'1900-01-01','Holder'
					FROM custacct
					--WHERE hldorjnt in('J','S')
					--WHERE hldorjnt not in('H')			-- #10354 -- include hldorjnt 'R'
					WHERE hldorjnt = 'J'					 --#13949
				END
									
				-- Get accounts to check   -- #8478
				IF @piProcess='L' AND @AccTypeToQualify!='Both'
				BEGIN
					INSERT INTO #AcctsToCheck (acctno,accttype) 
					SELECT a.acctno,a.accttype 
					FROM acct a 
					INNER JOIN custacct ca ON a.acctno = ca.acctno AND ca.hldorjnt='H'
					INNER JOIN #instantcredit i ON i.Hcustid=ca.custid
					WHERE (accttype='O' AND HAcctType in ('HP'))
					OR (accttype='R' AND HAcctType in ('RF'))
				
					SET @AcctsToCheck=1
					--- Addex index suggested by christian
					CREATE NONCLUSTERED INDEX [IX_AcctsToCheck_acctno] ON #AcctsToCheck
					( [acctno] ASC)
				END
				-- get the Main account holder for joint accounts
				UPDATE #JointAccounts
				SET Hcustid=ca.custid
				FROM #JointAccounts j 
				INNER JOIN custacct ca ON j.acctno=ca.acctno
				WHERE hldorjnt='H'

				-- get the date account opened
				UPDATE #JointAccounts
				SET dateacctopen=a.dateacctopen
				FROM #JointAccounts j 
				INNER JOIN acct a ON j.acctno=a.acctno

				-- update instant credit with joint holder
				UPDATE i
				SET Jcustid=j.custid	
				FROM #InstantCredit i 
				INNER JOIN #JointAccounts j on i.Hcustid=j.Hcustid
				WHERE dateacctopen=(SELECT MAX(dateacctopen) 
									FROM #JointAccounts j2 
									WHERE j.Hcustid=j2.Hcustid	-- jec 21/09/07
									AND j2.hcustid=i.hcustid   ) -- AND i.jcustid !='Joint'	-- jec 20/09/07

				-- AND again for main holder 
				UPDATE i
				SET Jcustid=j.Hcustid	
				FROM #InstantCredit i 
				INNER JOIN #JointAccounts j ON i.Hcustid=j.custid
				WHERE dateacctopen=(SELECT MAX(dateacctopen) 
									FROM #JointAccounts j2 
									WHERE j.Hcustid=j2.Hcustid	-- jec 21/09/07
									)  
				AND i.jcustid ='NoJoint'	-- jec 20/09/07

				-- Staff Accounts		-- #8463
				UPDATE #InstantCredit
				SET Staff='Y'
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Hcustid=ca.custid	AND ca.hldorjnt='H'
				INNER JOIN acct a ON ca.acctno = a.acctno
				--WHERE currstatus='9' OR highststatus='9'		-- #8619
				WHERE currstatus='9' --or highststatus='9'		--IP - 06/01/12 - #9356-- #8619
								
				--update customer AND joint score based on update of the most recent account proposal
				-- ;with maxDate(custid, dateprop,acctno)					-- #17649
				-- as 
				-- (
				--     SELECT custid, MAX(dateprop),MAX(acctno)				-- #17649
				--     FROM proposal p
				--     WHERE (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck WHERE acctno = p.acctno) OR @AcctsToCheck=0)
				--         AND p.propresult != ' '
				--and p.dateprop = (SELECT TOP 1 MAX(dateprop) FROM proposal p2 WHERE p.custid=p2.custid)				-- #17649
				--and p.acctno = (SELECT MAX(acctno) FROM proposal p3 WHERE p.custid=p3.custid AND p.dateprop=p3.dateprop AND p3.propresult!='X')		-- #17649
				--     GROUP BY custid
				-- )
				
				-- Copy only customer id repalted proposal into temp table.

				INSERT INTO #TempProposal
				SELECT custid, dateprop,acctno,points,datechange,propresult				
				FROM proposal P
				INNER JOIN #instantCredit I ON P.custid = I.Hcustid
				--- Index added as it was taking more time to execute the query
				CREATE INDEX ix_TempProposal_custid ON #TempProposal(custid) include(DateProp,Acctno)

				INSERT INTO #maxDate
				SELECT custid, MAX(dateprop) AS dateprop,MAX(acctno) as acctno				-- #17649
				FROM #TempProposal p
				WHERE (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck WHERE acctno = p.acctno) OR @AcctsToCheck=0)
				AND p.propresult != ' '
				AND p.dateprop = (SELECT TOP 1 MAX(dateprop) FROM #TempProposal p2 WHERE p.custid=p2.custid)				-- #17649
				AND p.acctno = (SELECT MAX(acctno) 
								FROM #TempProposal p3 
								WHERE p.custid=p3.custid AND p.dateprop=p3.dateprop AND p3.propresult!='X')		-- #17649
				GROUP BY custid
				CREATE INDEX ix_maxDate_custid ON #maxDate(custid)

				UPDATE i
				SET i.Hscore = (SELECT TOP 1 p.points  --#13948 --,Hdatechange=datechange jec 03/10/07
								FROM #TempProposal p 
								INNER JOIN #maxDate m ON p.custid = m.custid
									AND p.dateprop = m.dateprop
									AND p.dateprop = m.dateprop 
									AND p.acctno=m.acctno			-- #17649
								WHERE i.Hcustid=p.custid
								AND ISNULL(datechange,'1900-01-01')=(SELECT MAX(ISNULL(datechange,'1900-01-01')) -- isnull jec 20/09/07
																	FROM #TempProposal p2 
																	WHERE p2.custid=p.custid 
																	AND p2.dateprop = p.dateprop
																	AND p2.acctno=p.acctno		-- #17649
																	)
								)
				--,   
				--	i.jScore = (SELECT p.points					--#16883 - SELECT max datechange AND dateprop, as some customers had multiple entries with same datechange
				--        FROM proposal p 
				--            WHERE i.Jcustid=p.custid
				--            AND ISNULL(datechange,'1900-01-01')=(SELECT MAX(ISNULL(datechange,'1900-01-01')) -- isnull jec 20/09/07
				--                                        FROM proposal p2 
				--                                                    WHERE p2.custid=p.custid)
				--			and ISNULL(dateprop, '1900-01-01') = (SELECT MAX(ISNULL(dateprop,'1900-01-01')) 
				--								FROM proposal p3
				--									WHERE p3.custid = p.custid)
				--        )
				FROM #instantCredit i
				WHERE i.Staff != 'Y'

				-- #17649 - update of joint points split FROM above as was causing subquery error
				--- Below code was extra commented by Sunil
				--;with maxDate(custid, dateprop,acctno)					-- #17649
				--  as 
				--  (
				--      SELECT custid, MAX(dateprop),MAX(acctno)				-- #17649
				--      FROM proposal p
				--      WHERE (EXISTS(SELECT 'a' FROM #AcctsToCheck WHERE acctno = p.acctno) OR @AcctsToCheck=0)
				--          AND p.propresult != ' '
				--	and p.dateprop = (SELECT TOP 1 MAX(dateprop) FROM proposal p2 WHERE p.custid=p2.custid)				-- #17649
				--	and p.acctno = (SELECT MAX(acctno) FROM proposal p3 WHERE p.custid=p3.custid AND p.dateprop=p3.dateprop AND p3.propresult!='X')		-- #17649
				--      GROUP BY custid
				--  )

				UPDATE i
				SET i.jScore = ISNULL((SELECT p.points					--#16883 - SELECT max datechange AND dateprop, as some customers had multiple entries with same datechange
								FROM #TempProposal p 
								WHERE i.Jcustid=p.custid
								AND ISNULL(datechange,'1900-01-01')=(SELECT MAX(ISNULL(datechange,'1900-01-01')) -- isnull jec 20/09/07
																	FROM #TempProposal p2 
																	WHERE p2.custid=p.custid)
								AND ISNULL(dateprop, '1900-01-01') = (SELECT MAX(ISNULL(dateprop,'1900-01-01')) 
																	FROM #TempProposal p3
																	WHERE p3.custid = p.custid)
								AND p.acctno = (SELECT MAX(acctno) 
												FROM #TempProposal p3 
												WHERE p.custid=p3.custid 
												AND p.dateprop=p3.dateprop 
												AND p3.propresult!='X')		
								),0)
				FROM #instantCredit i
				WHERE i.Staff != 'Y'


				--    i.jScore = (SELECT TOP 1 p.points
				--               FROM proposal p 
				--                  INNER JOIN maxDate m
				--                  on p.custid = m.custid
				--                        AND p.dateprop = m.dateprop
				--                  WHERE i.Jcustid=p.custid
				--                 AND ISNULL(datechange,'1900-01-01')=(SELECT MAX(ISNULL(datechange,'1900-01-01')) -- isnull jec 20/09/07
				--                                                FROM proposal p2 
				--                                                         WHERE p2.custid=p.custid 
				--                                                            AND p2.dateprop = p.dateprop
				--                                                         )
				--             )
				--FROM #instantCredit i
				--WHERE i.Staff != 'Y'

				--#19353 - CR18831
				IF(@piProcess='I' AND ISNULL(@piCustomerID,' ')=' ')
				BEGIN
					--Instant Credit report only allows users to look at data
					--FROM the top 5 runs.
					--Therefore delete data FROM InstantCreditApprovalChecks older then the min date of the last 5 runs, 
					--due to this table getting very large.
					SELECT DISTINCT TOP 5 preapprovaldate
					INTO #Preapprovaldates
					FROM InstantCreditApprovalChecks
					ORDER BY preapprovaldate DESC

					DELETE 
					FROM InstantCreditApprovalChecks
					WHERE PreapprovalDate <= (SELECT MIN(preapprovaldate) FROM #Preapprovaldates)


					INSERT INTO InstantCreditApprovalChecks
					SELECT * FROM #instantcredit i
				END

				-- Now remove all customers that don't make minimum score - copying quicker than deleting
				IF (@piProcess='I' AND ISNULL(@piCustomerID,' ')=' ')		-- EOD mode
				BEGIN
					SELECT * 
					INTO #instantcredit_save 
					FROM #instantcredit 
					WHERE (hscore>=@MinCredScore 
					AND (((Jscore>=@MinCredScore OR Jscore=0) AND Jcustid!='NoJoint')	-- jec 20/09/07						
					OR Jcustid='NoJoint')
					OR Staff='Y')					-- Staff Account	-- #8463

					-- debugging only

					--			drop table #instantcredit_save_all		-- testing only
					--			SELECT * into #instantcredit_save_all FROM #instantcredit -- debugging only

					-- end debugging only
					TRUNCATE TABLE #instantcredit

					INSERT INTO #instantcredit 
					SELECT * FROM #instantcredit_save
					TRUNCATE TABLE #instantcredit_save
				END

			-- Now remove all customers that don't make minimum score - copying quicker than deleting
				IF (@piProcess='L' AND ISNULL(@piCustomerID,' ')=' ')		-- EOD mode
				BEGIN
					SELECT * 
					INTO #instantcreditloan_save 
					FROM #instantcredit 
					WHERE ((hscore>=@MinCredScore OR (hscore>=@MinCredScoreRecent) OR (hscore>=@MinCredScoreNew)) 
					and (((Jscore>=@MinCredScore OR (Jscore>=@MinCredScoreRecent) OR (Jscore>=@MinCredScoreNew) OR (Jscore=0)) AND Jcustid!='NoJoint')	-- jec 20/09/07						
					OR Jcustid='NoJoint')
					OR Staff='Y')					-- Staff Account	-- #8463

					-- debugging only
					--			drop table #instantcredit_save_all		-- testing only
					--			SELECT * into #instantcredit_save_all FROM #instantcredit -- debugging only
					-- end debugging only
					TRUNCATE TABLE #instantcredit

					INSERT INTO #instantcredit 
					SELECT * FROM #instantcreditloan_save
					TRUNCATE TABLE #instantcreditloan_save
				END

			-- Update RF % Avail		-- CR906
				IF @piProcess = 'L'	
				BEGIN
					-- Holder
					UPDATE #InstantCredit
					SET HRFpcAvail = FLOOR((c.AvailableSpend/c.RFCreditLimit) * 100)
					,JRFpcAvail = FLOOR((c.AvailableSpend/c.RFCreditLimit) * 100)
					FROM #InstantCredit i 
					--INNER JOIN customer c on i.Hcustid=c.custid
					INNER JOIN #CustomerForCreditUpdate c  ON i.Hcustid=c.custid  -- suggested by Javier Parada
					WHERE c.RFCreditLimit>0 
					-- jec 22/11/07 error occurs if AvailableSpend a lot greater than RFCreditLimit
					AND c.RFCreditLimit>=c.AvailableSpend	
					AND c.AvailableSpend > 0--RM 14/09/2010 an error also occurs if available spend v negative

					-- commented because it uses same condition hence consolidated in above update query
					-- Joint
					--UPDATE #InstantCredit
					--SET JRFpcAvail = FLOOR((c.AvailableSpend/c.RFCreditLimit) * 100)
					--FROM #InstantCredit i 
					----INNER JOIN customer c on i.Jcustid=c.custid
					--INNER JOIN #CustomerForCreditUpdate c  ON i.Jcustid=c.custid  -- suggested by Javier Parada
					--WHERE c.RFCreditLimit>0
					---- jec 22/11/07 error occurs if AvailableSpend a lot greater than RFCreditLimit
					--AND c.RFCreditLimit>=c.AvailableSpend	
					--AND c.AvailableSpend > 0 --RM 14/09/2010 an error also occurs if available spend v negative	

					-- CR1232 - %RF allocated to Cash Loan
					INSERT INTO #CashLoan (custid,OutstBalLoan)
					--SELECT Hcustid,SUM(LoanAmount)					--#8586 - sum loanamount for account not settled
					SELECT Hcustid,SUM(a.outstbal - ag.servicechg)		--#8645 - use outstanding balance. Any payments should increase amount available for Cash Loan
					FROM #InstantCredit i 
					INNER JOIN CashLoan l ON i.Hcustid=l.custid
					INNER JOIN acct a ON l.acctno=a.acctno
					INNER JOIN agreement ag ON a.acctno = ag.acctno	--#8645 
					WHERE a.currstatus!='S'
					GROUP BY HCustid

					UPDATE #InstantCredit
					SET HRFpcCashLoan= ISNULL(FLOOR((OutstBalLoan/c.RFCreditLimit) * 100),0)
					FROM #InstantCredit i 
					--INNER JOIN customer c on i.Hcustid=c.custid
					INNER JOIN #CustomerForCreditUpdate c  ON i.Hcustid=c.custid  -- suggested by Javier Parada
					INNER JOIN #CashLoan l ON i.Hcustid=l.custid
					WHERE c.RFCreditLimit>0	
				END

				-- get latest date authorised (Holder)
				INSERT INTO #DateAuth (custid,dateauth)
				SELECT Hcustid,MAX(dateauth)
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Hcustid=ca.custid AND hldorjnt ='H'
				INNER JOIN agreement a ON ca.acctno=a.acctno
				WHERE a.acctno!=@piAccountNo	-- not this account		jec 12/11/07
				GROUP BY Hcustid

				UPDATE #InstantCredit
				SET Hdatechange=dateauth
				FROM #InstantCredit i 
				INNER JOIN #DateAuth d ON i.Hcustid=d.custid

				TRUNCATE TABLE #DateAuth

				-- get latest date authorised (Joint) 
				INSERT INTO #DateAuth (custid,dateauth)
				SELECT Jcustid,MAX(dateauth)
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Jcustid=ca.custid AND hldorjnt ='H'
				INNER JOIN agreement a ON ca.acctno=a.acctno
				WHERE a.acctno!=@piAccountNo	-- not this account		jec 12/11/07
				GROUP BY Jcustid

				UPDATE #InstantCredit
				SET Jdatechange=dateauth
				FROM #InstantCredit i 
				INNER JOIN #DateAuth d ON i.Jcustid=d.custid

				-- get credit settled date for Holder
				INSERT INTO #settled (custid,datesettled)
				--SELECT i.Hcustid,MAX(datestatchge) 
				--FROM status s INNER JOIN custacct ca on s.acctno=ca.acctno AND hldorjnt='H'	and s.statuscode='S'	-- jec 24/10/07 uat 353
				SELECT i.Hcustid,MAX(datetrans) 
				FROM fintrans s 
				INNER JOIN custacct ca ON s.acctno=ca.acctno AND hldorjnt='H'	-- merge 5.13
				INNER JOIN  #InstantCredit i ON ca.custid=i.Hcustid 
				INNER JOIN acct a ON ca.acctno=a.acctno AND a.agrmttotal>0
				INNER JOIN agreement ag ON a.acctno = ag.acctno				
				--FROM status s,#InstantCredit i,custacct ca
				--WHERE s.statuscode='S' AND s.acctno=ca.acctno AND hldorjnt='H' AND ca.custid=i.Hcustid AND SUBSTRING(s.acctno,4,1)=0
				WHERE SUBSTRING(s.acctno,4,1)='0'
				AND a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
				AND a.currstatus = 'S'		-- merge 5.13
				AND ag.datedel!='1900-01-01'				-- #8476 A/c must have been delivered
				AND NOT EXISTS(SELECT TOP 1 'a' 
								FROM cancellation c 
								WHERE c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
								AND s.transtypecode = 'PAY'		-- merge 5.13
								AND (DATEDIFF(MONTH, ag.datedel, s.datetrans) >= @SettAcctLen) -- merge 5.13 CR1225 --IP - 07/03/11 - only check @SettAcctLen for InstantCredit
								--and (DATEDIFF(MONTH, ag.datedel, s.datestatchge) >= @SettAcctLen OR @piProcess = 'L') --CR1225 --IP - 07/03/11 - only check @SettAcctLen for InstantCredit
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478				
				GROUP BY i.Hcustid

				-- update date settled credit for Holder
				UPDATE #InstantCredit
				SET HdateSettCred=datesettled
				FROM #InstantCredit i 
				INNER JOIN #settled s ON i.Hcustid=s.custid

				TRUNCATE TABLE #settled 

				-- get cash settled date for Holder		
				--insert into #settled (custid,datesettled)					-- Merge 5.13 - removed
				--SELECT i.Hcustid,MAX(datestatchge)
				--FROM status s INNER JOIN custacct ca on s.acctno=ca.acctno AND hldorjnt='H'	and s.statuscode='S'	-- jec 24/10/07 uat 353
				--			INNER JOIN  #InstantCredit i on ca.custid=i.Hcustid
				-- 			INNER JOIN acct a on ca.acctno=a.acctno AND a.agrmttotal>0  
				----FROM status s,#InstantCredit i,custacct ca
				----WHERE s.statuscode='S' AND s.acctno=ca.acctno AND hldorjnt='H' AND ca.custid=i.Hcustid AND SUBSTRING(s.acctno,4,1)!=0
				--WHERE SUBSTRING(s.acctno,4,1)!='0'
				--			and a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
				--			and not EXISTS(SELECT * FROM cancellation c 
				--							WHERE c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
				--GROUP BY i.Hcustid
								
				-- update date settled credit for Holder
				--No Longer Used
				UPDATE #InstantCredit
				SET HdateSettcash='1900-01-01'		--SET HdateSettcash=datesettled    merge 5.13
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Hcustid=s.custid

				TRUNCATE TABLE #Settled 
								
				-- get credit settled date for Joint
				INSERT INTO #Settled (custid,datesettled)
				--SELECT i.Jcustid,MAX(datestatchge)
				--FROM status s INNER JOIN custacct ca on s.acctno=ca.acctno AND hldorjnt='J'	and s.statuscode='S'	-- jec 24/10/07 uat 353
				SELECT i.Jcustid,MAX(datetrans)
				FROM fintrans s 
				INNER JOIN custacct ca ON s.acctno=ca.acctno AND hldorjnt='J'		-- merge 5.13
				INNER JOIN  #InstantCredit i ON ca.custid=i.Hcustid
				INNER JOIN acct a ON ca.acctno=a.acctno AND a.agrmttotal>0  
				INNER JOIN agreement ag ON ag.acctno = a.acctno 
				--	FROM status s,#InstantCredit i,custacct ca
				--	WHERE s.statuscode='S' AND s.acctno=ca.acctno AND hldorjnt='J' AND ca.custid=i.Jcustid AND SUBSTRING(s.acctno,4,1)=0
				WHERE SUBSTRING(s.acctno,4,1)='0'
				AND a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
				AND a.currstatus = 'S'
				AND ag.datedel!='1900-01-01'				-- #8476 A/c must have been delivered
				AND s.transtypecode = 'PAY'				-- merge 5.13
				AND (DATEDIFF(MONTH, ag.datedel, s.datetrans) >= @SettAcctLen) -- merge 5.13 CR1225 --IP - 07/03/11 - only check @SettAcctLen for InstantCredit
				--and (DATEDIFF(MONTH, ag.datedel, s.datestatchge) >= @SettAcctLen OR @piProcess = 'L') --CR1225 --IP - 07/03/11 - only check @SettAcctLen for InstantCredit
				AND NOT EXISTS(SELECT TOP 1 'a' 
								FROM cancellation c 
								WHERE c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
				GROUP BY i.Jcustid

				-- update date settled credit for Joint
				UPDATE #InstantCredit
				SET JdateSettCred=datesettled
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Jcustid=s.custid

				TRUNCATE TABLE #Settled 

				-- get cash settled date for Joint
				--	insert into #Settled (custid,datesettled)				-- merge 5.13 removed
				--	SELECT i.Jcustid,MAX(datestatchge)
				--	FROM status s INNER JOIN custacct ca on s.acctno=ca.acctno AND hldorjnt='J'	and s.statuscode='S'	-- jec 24/10/07 uat 353
				--				INNER JOIN  #InstantCredit i on ca.custid=i.Hcustid 
				-- 				INNER JOIN acct a on ca.acctno=a.acctno AND a.agrmttotal>0  
				----	FROM status s,#InstantCredit i,custacct ca
				----	WHERE s.statuscode='S' AND s.acctno=ca.acctno AND hldorjnt='J' AND ca.custid=i.Jcustid AND SUBSTRING(s.acctno,4,1)!=0
				--	WHERE SUBSTRING(s.acctno,4,1)!='0'
				--				and a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
				--				and not EXISTS(SELECT * FROM cancellation c 
				--								WHERE c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
				--	GROUP BY i.Jcustid
								
				-- update date settled credit for Joint
				--No Longer Used
				UPDATE #InstantCredit
				SET JdateSettcash='1900-01-01'		-- SET JdateSettcash=datesettled   merge 5.13
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Jcustid=s.custid

				-- update current account length for Holder (use earliest date delivered)
				INSERT INTO #Delivered (custid,dateDelivered)
				SELECT i.Hcustid,MIN(datedel) 
				FROM agreement ag 
				INNER JOIN acct a ON ag.acctno=a.acctno 
				INNER JOIN custacct ca ON a.acctno=ca.acctno 
				INNER JOIN #InstantCredit i ON ca.custid=i.Hcustid
				WHERE hldorjnt='H' AND datedel!='1900-01-01' AND currstatus!='S'
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				--RM 8/02/10 need to only check credit accounts currently open
				AND SUBSTRING(a.acctno,4,1)='0'	
				GROUP BY i.Hcustid

				UPDATE #InstantCredit
				SET HexistAccLen=DATEDIFF(mm,dateDelivered,GETDATE())
				FROM #InstantCredit i 
				INNER JOIN #Delivered d ON i.Hcustid=d.custid

				--SELECT Customers settled accounts AND Term Lengths
				INSERT INTO #SettledDelivered (Custid,DateDelivered, TermLength, DateSettled)
				SELECT i.Hcustid, datedel, ip.instalno, s.datestatchge
				FROM agreement ag 
				INNER JOIN acct a ON ag.acctno=a.acctno 
				INNER JOIN custacct ca ON a.acctno=ca.acctno 
				INNER JOIN instalplan ip ON ip.acctno = ca.acctno AND ip.agrmtno = ag.agrmtno
				INNER JOIN #InstantCredit i ON ca.custid=i.Hcustid
				INNER JOIN status s ON a.acctno = s.acctno
				WHERE hldorjnt='H' AND datedel!='1900-01-01' AND currstatus='S'
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				AND s.statuscode = 'S'
				AND s.id = (SELECT TOP 1 MAX(id)
							FROM status s1
							WHERE s1.acctno = s.acctno
							AND s1.statuscode = 'S'
							GROUP BY s1.datestatchge
							ORDER BY s1.datestatchge DESC
							)
				--and s.statuscode = 'S'
				--and s.datestatchge = (SELECT MAX(s1.datestatchge)
				--FROM status s1
				--WHERE s1.acctno = s.acctno
				--and s1.statuscode = 'S')
				--RM 8/02/10 need to only check credit accounts currently open
				AND SUBSTRING(a.acctno,4,1)='0'	

				--SELECT Customers Settled / Un-settled accounts (use earliest date delivered)
				INSERT INTO #DeliveredBoth (Custid,DateDelivered)
				SELECT i.Hcustid,MIN(datedel) 
				FROM agreement ag 
				INNER JOIN acct a ON ag.acctno=a.acctno 
				INNER JOIN custacct ca ON a.acctno=ca.acctno 
				INNER JOIN #InstantCredit i ON ca.custid=i.Hcustid
				WHERE hldorjnt='H' AND datedel!='1900-01-01'
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				--RM 8/02/10 need to only check credit accounts currently open
				AND SUBSTRING(a.acctno,4,1)='0'	
				GROUP BY i.Hcustid

				--Update Holder Customer as New / Recent / Existing Customer

				--New Customers
				UPDATE #InstantCredit
				SET HNewCustomer = 1
				FROM #InstantCredit i
				WHERE NOT EXISTS (SELECT TOP 1 'a' 
								FROM delivery d
								INNER JOIN  Custacct ca ON i.Hcustid = ca.custid AND ca.hldorjnt = 'H'
								WHERE d.acctno = ca.acctno) 
				AND Jcustid != 'Joint'

				--Existing Customer 
				--Customers first delivery on unsettled account >= @ExistAcctLen months ago
				UPDATE #InstantCredit
				SET HExistingCustomer = 1
				FROM #InstantCredit i 
				INNER JOIN #Delivered d ON i.Hcustid=d.custid
				WHERE
				--DATEDIFF(mm,dateDelivered,GETDATE()) >= @ExistAcctLen
				dateDelivered <= DATEADD(mm, -@ExistAcctLen, GETDATE()) --more than OR equal to x months ago
				AND HNewCustomer != 1                                       ---some accounts had their datedel SET on agreement but no delivery (takeon) therefore should be New
				AND Jcustid != 'Joint'  

				--Or Customer has Settled account delivered >= @ExistAcctLen months ago AND account has a Term Length >= @SettledMinTermLength parameter
				--and the settled account was settled >= @SettCredMonths
				UPDATE #InstantCredit
				SET HExistingCustomer = 1
				FROM #InstantCredit i
				INNER JOIN #SettledDelivered sd ON i.Hcustid = sd.custid
				WHERE
				--DATEDIFF(mm,dateDelivered,GETDATE()) >= @ExistAcctLen 
				dateDelivered <= DATEADD(mm, -@ExistAcctLen, GETDATE())
				AND sd.TermLength >= @SettledMinTermLength  
				AND sd.DateSettled >= DATEADD(mm, -@SettCredMonths, GETDATE()) 
				AND HNewCustomer != 1
				AND Jcustid != 'Joint'
				
				--Recent Customer
				--Customers first delivery on Unsettled / Settled account was between @RecentCustAcctLen AND @ExistAcctLen months ago
				UPDATE #InstantCredit 
				SET HRecentCustomer = 1
				FROM #InstantCredit i
				INNER JOIN #DeliveredBoth db ON i.Hcustid = db.custid
				WHERE dateDelivered > DATEADD(mm, -@ExistAcctLen, GETDATE())
				AND dateDelivered < DATEADD(mm, -@RecentCustAcctLen, GETDATE())
				AND Jcustid != 'Joint'
				--DATEDIFF(mm,dateDelivered,GETDATE()) > @RecentCustAcctLen
				--AND DATEDIFF(mm,dateDelivered,GETDATE())  < @ExistAcctLen

				TRUNCATE TABLE #Delivered
				-- update current account length for Joint
				INSERT INTO #Delivered (custid,dateDelivered)
				SELECT i.Jcustid,MIN(datedel) 
				FROM agreement ag 
				INNER JOIN acct a ON ag.acctno=a.acctno 
				INNER JOIN custacct ca ON a.acctno=ca.acctno 
				INNER JOIN #InstantCredit i ON ca.custid=i.Jcustid
				WHERE hldorjnt='H' AND datedel!='1900-01-01' AND currstatus!='S'        --Changed FROM 'J' to 'H' as this was not setting account length
				GROUP BY i.Jcustid

				UPDATE #InstantCredit
				SET JExistAccLen=DATEDIFF(mm,dateDelivered,GETDATE())
				FROM #InstantCredit i 
				INNER JOIN #Delivered d ON i.Jcustid=d.custid 

				TRUNCATE TABLE #SettledDelivered

				--SELECT Joint Customers settled accounts AND Term Lengths 
				INSERT INTO #SettledDelivered (Custid,DateDelivered, TermLength, DateSettled)
				SELECT i.Jcustid,datedel,ip.instalno, s.datestatchge
				FROM agreement ag 
				INNER JOIN acct a ON ag.acctno=a.acctno 
				INNER JOIN custacct ca ON a.acctno=ca.acctno 
				INNER JOIN instalplan ip ON ip.acctno = ca.acctno AND ip.agrmtno = ag.agrmtno
				INNER JOIN #InstantCredit i ON ca.custid=i.Jcustid
				INNER JOIN status s ON a.acctno = s.acctno
				WHERE hldorjnt='H' AND datedel!='1900-01-01' AND currstatus='S'
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				AND s.statuscode = 'S'
				AND s.id = (SELECT TOP 1 MAX(id)
							FROM status s1
							WHERE s1.acctno = s.acctno
							AND s1.statuscode = 'S'
							GROUP BY s1.datestatchge
							ORDER BY s1.datestatchge DESC)
				--and s.statuscode = 'S'
				--and s.datestatchge = (SELECT MAX(s1.datestatchge)
				--FROM status s1
				--WHERE s1.acctno = s.acctno
				--and s1.statuscode = 'S')
				--RM 8/02/10 need to only check credit accounts currently open
				AND SUBSTRING(a.acctno,4,1)='0'	

				TRUNCATE TABLE #DeliveredBoth

				--SELECT Joint Customers Settled / Un-settled accounts (use earliest date delivered)
				INSERT INTO #DeliveredBoth (Custid,DateDelivered)
				SELECT i.Jcustid,MIN(datedel) 
				FROM agreement ag 
				INNER JOIN acct a ON ag.acctno=a.acctno 
				INNER JOIN custacct ca ON a.acctno=ca.acctno 
				INNER JOIN #InstantCredit i ON ca.custid=i.Jcustid
				WHERE hldorjnt='H' AND datedel!='1900-01-01'
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				--RM 8/02/10 need to only check credit accounts currently open
				AND SUBSTRING(a.acctno,4,1)='0'	
				GROUP BY i.Jcustid

				--Update Joint Customer as New / Recent / Existing Customer

				--New Customers
				UPDATE #InstantCredit
				SET JNewCustomer = 1
				FROM #InstantCredit i
				WHERE NOT EXISTS (SELECT TOP 1 'A' 
									FROM delivery d
									INNER JOIN Custacct ca ON i.Jcustid = ca.custid AND ca.hldorjnt = 'H'
									WHERE d.acctno = ca.acctno) 
				AND i.Jcustid != 'NoJoint'
				AND i.Jcustid != 'Joint'

				--Existing Customer 
				--Customers first delivery on Unsettled account >= @ExistAcctLen months ago
				UPDATE #InstantCredit
				SET JExistingCustomer = 1
				FROM #InstantCredit i 
				INNER JOIN #Delivered d ON i.Jcustid=d.custid
				WHERE dateDelivered <= DATEADD(mm, -@ExistAcctLen, GETDATE()) --more than OR equal to x months ago
				--DATEDIFF(mm,dateDelivered,GETDATE()) >= @ExistAcctLen
				AND i.Jcustid != 'NoJoint'
				AND i.JNewCustomer != 1

				--Or Joint Customer has settled account delivered >= @ExistAcctLen months ago AND account has a Term Length >= @SettledMinTermLength parameter
				--and the settled account was settled >= @SettCredMonths
				UPDATE #InstantCredit
				SET JExistingCustomer = 1
				FROM #InstantCredit i
				INNER JOIN #SettledDelivered sd ON i.Jcustid = sd.custid
				WHERE
				--DATEDIFF(mm,dateDelivered,GETDATE()) >= @ExistAcctLen 
				dateDelivered <= DATEADD(mm, -@ExistAcctLen, GETDATE())
				AND sd.TermLength >= @SettledMinTermLength  
				AND sd.DateSettled >= DATEADD(mm, -@SettCredMonths, GETDATE()) 
				AND i.Jcustid != 'NoJoint'
				AND i.JNewCustomer != 1

				--Recent Customer
				--Joint Customers first delivery on unsettled / settled account was between @RecentCustAcctLen AND @ExistAcctLen months ago
				UPDATE #InstantCredit 
				SET JRecentCustomer = 1
				FROM #InstantCredit i
				INNER JOIN #DeliveredBoth db ON i.Jcustid = db.custid
				WHERE dateDelivered > DATEADD(mm, -@ExistAcctLen, GETDATE())
				AND dateDelivered < DATEADD(mm, -@RecentCustAcctLen, GETDATE())
				--DATEDIFF(mm,dateDelivered,GETDATE()) > @RecentCustAcctLen
				--and DATEDIFF(mm,dateDelivered,GETDATE())  < @ExistAcctLen
				AND i.Jcustid != 'NoJoint'

				-- Update Highest status Holder was settled now includes live accounts
				INSERT INTO #Settled (custid,datesettled,status)
				SELECT i.Hcustid, NULL, MAX(highststatus) 
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Hcustid=ca.custid 
				INNER JOIN acct a ON a.acctno=ca.acctno
				INNER JOIN status s ON a.acctno=s.acctno
				WHERE hldorjnt='H' 
				AND statuscode NOT IN ('U','O','0')
				AND datestatchge>=DATEADD(m,@HighStatusTimeFrame * -1,GETDATE())
				AND a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
				AND NOT EXISTS(SELECT TOP 1 'a' 
								FROM cancellation c 
								WHERE c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
				AND (EXISTS(SELECT TOP 1 'a'  FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				GROUP BY i.Hcustid

				UPDATE #InstantCredit
				SET HhighSettStatus=status
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Hcustid=s.custid

				TRUNCATE TABLE #Settled 

				-- Update Highest settled status Joint was settled now includes live accounts
				INSERT INTO #Settled (custid,datesettled,status)
				SELECT i.Jcustid, NULL, MAX(highststatus) 
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Jcustid=ca.custid 
				INNER JOIN acct a ON a.acctno=ca.acctno
				INNER JOIN status s ON a.acctno=s.acctno
				WHERE hldorjnt='H' AND s.statuscode NOT IN ('U','O','0')	-- jec 21/09/07 hldorjnt 'J'
				AND datestatchge>=DATEADD(m,@HighStatusTimeFrame * -1,GETDATE())
				AND a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
				AND NOT EXISTS(SELECT TOP 1 'a' FROM cancellation c 
				WHERE c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
				GROUP BY i.Jcustid

				UPDATE #InstantCredit
				SET JHighSettStatus=status
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Jcustid=s.custid
								
				TRUNCATE TABLE #Settled			-- jec 21/09/07

				-- Update Highest status for Current accounts - Holder
				INSERT INTO #Settled (custid,datesettled,status)
				--SELECT i.Hcustid, NULL, MAX(highststatus)
				SELECT i.Hcustid, NULL, MAX(statuscode)			-- #3900 
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Hcustid=ca.custid 
				INNER JOIN acct a ON a.acctno=ca.acctno
				INNER JOIN status s ON a.acctno=s.acctno				-- #3900			
				WHERE hldorjnt='H' AND currstatus NOT IN('S','U')
				AND datestatchge>=DATEADD(m,@HighStatusTimeFrame * -1,GETDATE())		-- #3900
				AND statuscode NOT IN('U','O','S')			-- #8532 #3900
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478		
				GROUP BY i.Hcustid

				-- update WHERE currstatus>settled status
				UPDATE #InstantCredit
				SET HhighSettStatus=status
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Hcustid=s.custid
				WHERE status>HhighSettStatus

				TRUNCATE TABLE #Settled 

				-- Update Highest status for Current accounts - Joint
				INSERT INTO #Settled (custid,datesettled,status)
				--SELECT i.Jcustid, NULL, MAX(highststatus)
				SELECT i.Jcustid, NULL, MAX(statuscode)			-- #3900 
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Jcustid=ca.custid 
				INNER JOIN acct a ON a.acctno=ca.acctno
				INNER JOIN status s ON a.acctno=s.acctno				-- #3900			
				WHERE hldorjnt='H' AND currstatus NOT IN('S','U')		-- jec 21/09/07 hldorjnt 'J'
				AND datestatchge>=DATEADD(m,@HighStatusTimeFrame * -1,GETDATE())		-- #3900
				AND statuscode NOT IN('U','O','S')		-- #8532
				GROUP BY i.Jcustid

				-- update WHERE currstatus>settled status
				UPDATE #InstantCredit
				SET JhighSettStatus=status
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Jcustid=s.custid
				WHERE status>JhighSettStatus
								
				TRUNCATE TABLE #Settled 

				-- Update highest status of Holder WHERE also a Joint 
				INSERT INTO #Settled (custid,datesettled,status)
				SELECT i.Jcustid, NULL, JhighSettStatus 
				FROM #InstantCredit i
				WHERE EXISTS (SELECT TOP 1 i2.Hcustid FROM #InstantCredit i2 WHERE i.Jcustid=i2.Hcustid)

				UPDATE #InstantCredit 
				SET HhighSettStatus=status
				FROM #InstantCredit i 
				INNER JOIN #Settled s ON i.Hcustid=s.custid
				WHERE status > HhighSettStatus

				-- Get spouse accounts
				IF ISNULL(@piCustomerID,' ')!=' '
				BEGIN
					-- Get spouse accounts - Single Customer
					INSERT INTO #SpouseAccounts (custid,acctno,hldorjnt,Hcustid)
					SELECT custid,acctno,hldorjnt,'Holder' 
					FROM custacct ca
					WHERE hldorjnt ='S'
					AND EXISTS (SELECT TOP 1 'a' FROM custacct ca2 WHERE ca.acctno=ca2.acctno AND ca2.hldorjnt='H' AND ca2.custid=@piCustomerID)
				END
				ELSE
				BEGIN
					-- Get spouse accounts - All Customer (EOD)
					INSERT INTO #SpouseAccounts (custid,acctno,hldorjnt,Hcustid)
					SELECT custid,acctno,hldorjnt,'Holder'
					FROM custacct
					--WHERE hldorjnt in('S','J')	--'S'
					WHERE hldorjnt = 'S'	--#13949
				END

				-- Update with holder id
				UPDATE #SpouseAccounts
				SET Hcustid=ca.custid
				FROM #SpouseAccounts s 
				INNER JOIN custacct ca ON s.acctno=ca.acctno AND ca.hldorjnt='H'
				-- get accounts WHERE Spouse is holder
				INSERT INTO #SpouseAccounts (custid,acctno,hldorjnt)
				SELECT ca.custid,ca.acctno,ca.hldorjnt
				FROM custacct ca 
				INNER JOIN #SpouseAccounts s ON ca.custid=s.custid
				WHERE ca.hldorjnt='H' AND s.hldorjnt = 'S' -- #13949 AND s.hldorjnt in('S','J')	--='S' 
								
				-- get status of spouses accounts current
				UPDATE #SpouseAccounts
				SET currstatus=a.currstatus
				FROM #SpouseAccounts s 
				INNER JOIN acct a ON s.acctno=a.acctno
				WHERE a.currstatus !='S'	-- s.hldorjnt ='H' and
				
				-- get status of spouses accounts settled
				UPDATE #SpouseAccounts
				SET Highstatus=a.highststatus
				FROM #SpouseAccounts s INNER JOIN acct a ON s.acctno=a.acctno
				WHERE a.currstatus NOT IN ('U','0','O')		--  s.hldorjnt ='H' and

				UPDATE #InstantCredit
				SET HspouseBDWlegal='Y'
				FROM #InstantCredit i 
				INNER JOIN #SpouseAccounts s ON i.Hcustid=s.custid			
				WHERE s.currstatus IN('6','7','8') -- legal/BDW
				OR s.highstatus IN ('6','7','8')

				-- Update Spouse legal/bdw joint
				UPDATE #InstantCredit
				SET JspouseBDWlegal='Y'
				FROM #InstantCredit i 
				INNER JOIN #SpouseAccounts s ON i.Jcustid=s.custid 
				WHERE s.currstatus IN('6','7','8') -- legal/BDW
				OR s.highstatus IN ('6','7','8')

				-- Get Arrears Level Holder accts
				INSERT INTO #ArrearsLevel (custid,acctno,hldorjnt,ArrearsLevel)
				SELECT i.Hcustid,a.acctno,ca.hldorjnt,a.Arrears/ip.instalamount
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Hcustid=ca.custid AND ca.hldorjnt IN ('H')
				INNER JOIN acct a ON ca.acctno=a.acctno 
				INNER JOIN instalplan ip ON a.acctno=ip.acctno
				WHERE ip.instalamount>0
				AND (EXISTS(SELECT TOP 1 'a' FROM #AcctsToCheck ck WHERE ck.acctno=a.acctno) OR @AcctsToCheck=0)		-- #8478
				-- Get Arrears Level Joint accts
				INSERT INTO #ArrearsLevel (custid,acctno,hldorjnt,ArrearsLevel)
				SELECT i.Jcustid,a.acctno,ca.hldorjnt,a.Arrears/ip.instalamount
				FROM #InstantCredit i 
				INNER JOIN custacct ca ON i.Jcustid=ca.custid AND ca.hldorjnt = 'J' --#13949 ca.hldorjnt in ('J','S')	--jec 25/09/07
				INNER JOIN acct a ON ca.acctno=a.acctno 
				INNER JOIN instalplan ip ON a.acctno=ip.acctno
				WHERE ip.instalamount>0

				-- Update Holder
				UPDATE #InstantCredit
				SET HmaxArrearsLevel=ArrearsLevel
				FROM #InstantCredit i 
				INNER JOIN #ArrearsLevel a ON i.Hcustid=a.custid AND a.hldorjnt = 'H'
				WHERE a.ArrearsLevel=(SELECT MAX(a2.ArrearsLevel) FROM #ArrearsLevel a2 
									WHERE i.Hcustid=a2.custid AND a2.hldorjnt = 'H')			-- jec  25/09/07 a2.hldorjnt
				-- Update Joint
				UPDATE #InstantCredit
				SET JmaxArrearsLevel=ArrearsLevel
				FROM #InstantCredit i 
				INNER JOIN #ArrearsLevel a ON i.Jcustid=a.custid AND a.hldorjnt = 'J' --#13949 a.hldorjnt in('J','S')	--jec 25/09/07
				WHERE a.ArrearsLevel=(SELECT MAX(a2.ArrearsLevel) FROM #ArrearsLevel a2 
									WHERE i.Jcustid=a2.custid AND a2.hldorjnt = 'J')--#13949 a2.hldorjnt in('J','S'))			--jec 25/09/07 a2.hldorjnt ,'S'

				--IP - CR983 - 06/03/09 - Instant Credit - Update the 'HCustAddDateChange','JCustAddDateChange'
				--and 'HEmploymentDateChange' AND 'JEmploymentDateChange'.
				--IP - 22/02/11 - Sprint 5.10 - #3117
				UPDATE #InstantCredit
				SET HCustAddDateChange = ISNULL(ca.datechange, '1900-01-01')
				FROM #InstantCredit ic 
				INNER JOIN custaddress ca ON ic.Hcustid = ca.custid
				WHERE ca.datemoved IS NULL AND ca.addtype='H'
								
				UPDATE #InstantCredit
				SET JCustAddDateChange = ISNULL(ca.datechange, '1900-01-01')
				FROM #InstantCredit ic 
				INNER JOIN custaddress ca ON ic.Jcustid = ca.custid
				WHERE ca.datemoved IS NULL AND ca.addtype='H'
								
				UPDATE #InstantCredit
				SET HEmploymentDateChange =  ISNULL(e.datechanged, '1900-01-01')
				FROM #InstantCredit ic 
				INNER JOIN employment e ON ic.Hcustid = e.custid
				WHERE e.dateleft IS NULL
								
				UPDATE #InstantCredit
				SET JEmploymentDateChange = ISNULL(e.datechanged, '1900-01-01')
				FROM #InstantCredit ic 
				INNER JOIN employment e ON ic.Jcustid = e.custid
				WHERE e.dateleft IS NULL
									
				IF ISNULL(@piCustomerID,' ')=' '		-- EOD mode
				BEGIN
					--IP - 22/02/11 - Sprint 5.10 - #3117	
					UPDATE InstantCreditApprovalChecks
					SET HCustAddDateChange = ISNULL(ca.datechange, '1900-01-01')
					FROM InstantCreditApprovalChecks ic 
					INNER JOIN custaddress ca ON ic.Hcustid = ca.custid
					WHERE ca.datemoved IS NULL AND ca.addtype='H'
								
					UPDATE InstantCreditApprovalChecks
					SET JCustAddDateChange = ISNULL(ca.datechange, '1900-01-01')
					FROM InstantCreditApprovalChecks ic 
					INNER JOIN custaddress ca ON ic.Jcustid = ca.custid
					WHERE ca.datemoved IS NULL AND ca.addtype='H'
								
					UPDATE InstantCreditApprovalChecks
					SET HEmploymentDateChange = ISNULL(e.datechanged, '1900-01-01')
					FROM InstantCreditApprovalChecks ic 
					INNER JOIN employment e ON ic.Hcustid = e.custid
					WHERE e.dateleft IS NULL
								
					UPDATE InstantCreditApprovalChecks
					SET JEmploymentDateChange = ISNULL(e.datechanged, '1900-01-01')
					FROM InstantCreditApprovalChecks ic 
					INNER JOIN employment e ON ic.Jcustid = e.custid
					WHERE e.dateleft IS NULL
								
				END

				IF(@piProcess = 'I') --CR1225
				BEGIN
									
					IF(@ReferralMonths!=0) --IP - 04/03/11 - #3194 - Only if referral months > 0 then do the following check.
					BEGIN
					--IF (@piAccountNo != '' AND @referralmonths = 0 ) --IP - 04/03/11 - #3194 - Replaced with below
						IF(@piAccountNo != '') --IP - 04/03/11 - #3194
						BEGIN
							-- SET Referred - Holder
							UPDATE #InstantCredit
							SET HReferred='Y'
							FROM #InstantCredit i 
							INNER JOIN Referral r ON i.Hcustid=r.custid
							WHERE DATEDIFF(MONTH, r.datereferral, GETDATE()) <= @ReferralMonths -- #3895
							--FROM #InstantCredit i INNER JOIN proposal p on i.Hcustid=p.custid
							--WHERE ((p.Systemrecommendation in('R','X')) OR (p.SystemRecommendation='' AND p.propresult IN ('R','D')) )	-- ='R'
							--AND DATEDIFF(MONTH, p.datechange, GETDATE()) <= @ReferralMonths --CR1225
							--AND p.acctno = @piAccountNo
												
							-- SET Referred - Joint
							UPDATE #InstantCredit
							SET JReferred='Y'
							FROM #InstantCredit i 
							INNER JOIN Referral r ON i.Jcustid=r.custid
							WHERE DATEDIFF(MONTH, r.datereferral, GETDATE()) <= @ReferralMonths -- #3895
							--FROM #InstantCredit i INNER JOIN proposal p on i.Jcustid=p.custid
							--WHERE ((p.Systemrecommendation in('R','X')) OR (p.SystemRecommendation='' AND p.propresult IN ('R','D')) )
							--AND DATEDIFF(MONTH, p.datechange, GETDATE()) <= @ReferralMonths --CR1225
							--AND p.acctno = @piAccountNo
						END
						ELSE
						BEGIN
							-- SET Referred - Holder
							UPDATE #InstantCredit
							SET HReferred='Y'
							FROM #InstantCredit i 
							INNER JOIN Referral r ON i.Hcustid=r.custid
							WHERE DATEDIFF(MONTH, r.datereferral, GETDATE()) <= @ReferralMonths -- #3895
							and @referralmonths!=0		-- #8628
							--FROM #InstantCredit i INNER JOIN proposal p on i.Hcustid=p.custid
							--WHERE ((p.Systemrecommendation in('R','X')) OR (p.SystemRecommendation='' AND p.propresult IN ('R','D')) )	-- ='R'
							--AND DATEDIFF(MONTH, p.datechange, GETDATE()) <= @ReferralMonths --CR1225
												
							-- SET Referred - Joint
							UPDATE #InstantCredit
							SET JReferred='Y'
							FROM #InstantCredit i 
							INNER JOIN Referral r ON i.Jcustid=r.custid
							WHERE DATEDIFF(MONTH, r.datereferral, GETDATE()) <= @ReferralMonths -- #3895
							AND @referralmonths!=0		-- #8628
							--FROM #InstantCredit i INNER JOIN proposal p on i.Jcustid=p.custid
							--WHERE ((p.Systemrecommendation in('R','X')) OR (p.SystemRecommendation='' AND p.propresult IN ('R','D')) )
							--AND DATEDIFF(MONTH, p.datechange, GETDATE()) <= @ReferralMonths --CR1225
						END
					END
				END
				ELSE
				BEGIN
					-- SET Referred - Holder
					UPDATE #InstantCredit
					SET HReferred='Y'
					FROM #InstantCredit i 
					INNER JOIN Referral r ON i.Hcustid=r.custid
					WHERE DATEDIFF(MONTH, r.datereferral, GETDATE()) <= @ReferralMonths -- #3895
					AND @ReferralMonths!=0		-- #8628

					--FROM #InstantCredit i INNER JOIN proposal p on i.Hcustid=p.custid
					--WHERE p.Systemrecommendation in('R','X')	-- ='R'
					--		AND DATEDIFF(MONTH, p.datechange, GETDATE()) <= @ReferralMonths  AND @ReferralMonths!=0  -- #3895
					--		and (EXISTS(SELECT * FROM #AcctsToCheck ck WHERE ck.acctno=p.acctno) OR @AcctsToCheck=0)		-- #8478
									
					-- SET Referred - Joint
					UPDATE #InstantCredit
					SET JReferred='Y'
					FROM #InstantCredit i 
					INNER JOIN Referral r ON i.Jcustid=r.custid
					WHERE DATEDIFF(MONTH, r.datereferral, GETDATE()) <= @ReferralMonths -- #3895
					AND @ReferralMonths!=0		-- #8628
					--FROM #InstantCredit i INNER JOIN proposal p on i.Jcustid=p.custid
					--		AND DATEDIFF(MONTH, p.datechange, GETDATE()) <= @ReferralMonths  AND @ReferralMonths!=0  -- #3895
					--WHERE p.Systemrecommendation in('R','X')	-- ='R'
				END

				-- Now update Customer Instant credit OR Loan Qualified flag 
				IF ISNULL(@piCustomerID,' ')=' '
				BEGIN
					-- insert into Letter file (use acctno with latest dateacctopen)	-- CR906 moved
					DECLARE @letterdate DATETIME
					SET @letterdate=(SELECT GETDATE())

					-- Instant credit Processing
					IF @piProcess = 'I'				-- CR906
					BEGIN 
						/* Removing this for performance reasons - updating below AA
						Update Customer 
						SET InstantCredit='N'		-- ReSET Instant Credit first
						WHERE InstantCredit='Y' */
										
						--CR1225 if joint turned off SET no joint customer
						IF @jointqualification = 'false'
							UPDATE #instantcredit
						SET Jcustid ='NoJoint'		

						UPDATE #InstantCredit
						SET InstantCredit='Y'		-- Qualifies for Instant Credit	
						FROM #InstantCredit i			-- jec 21/11/07
						WHERE Hscore>=@MinCredScore 
						AND (HexistAccLen>=@ExistAcctLen OR DATEADD(mm,@SettCredMonths,ISNULL(Hdatesettcred,'1900-01-01'))>=GETDATE()
						--or DATEADD(mm,@SettCashMonths,ISNULL(Hdatesettcash,'1900-01-01'))>=GETDATE()
							)
						AND HhighsettStatus<=@maxstatus    -- also using current status UAT 195
						--and HhighsettStatus!='0'		-- #8532 -- status will be zero if only cancelled a/cs 69427 jec 14/12/07
						AND ISNULL(Hreferred,' ')!='Y'
						AND ISNULL(HspouseBDWlegal,' ')!='Y'
						AND HmaxArrearsLevel<=@MaxArrearsLevel
						AND ((Hdatechange>HCustAddDateChange
							--and DATEDIFF(MONTH, HCustAddDateChange , GETDATE()) <= @addresscheckmonths --IP - 01/03/11 - #3247 - Replaced with below
							AND HCustAddDateChange >= DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
							AND @ResidenceChangeApp='true'
							AND Hscore <= @addresscheckscore) 
							OR Hscore > @addresscheckscore
							--or DATEDIFF(MONTH, HCustAddDateChange , GETDATE()) > @addresscheckmonths --IP - 01/03/11 - #3247 - Replaced with below
							OR HCustAddDateChange < DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
							OR @ResidenceChangeApp='false')	-- jec 29/08/07
						AND((Hdatechange> HEmploymentDateChange
							AND @EmployChangeApp='true'
							--and DATEDIFF(MONTH,  HEmploymentDateChange, GETDATE()) <= @addresscheckmonths --IP - 28/02/11 - #3237 - Replaced with below
							AND HEmploymentDateChange >= DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237 
							AND Hscore <= @employmentcheckscore) 
						OR Hscore > @employmentcheckscore 
						--or DATEDIFF(MONTH,  HEmploymentDateChange, GETDATE()) > @employmentcheckmonths --IP - 28/02/11 - #3237 - Replaced with below
						OR HEmploymentDateChange < DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
						OR @EmployChangeApp='false')	-- jec 29/08/07
						AND (Jcustid ='NoJoint'					-- No Joint Customer
						OR (Jcustid !='NoJoint' AND Jscore=0)	-- Joint Customer with no own accounts - 20/09/07
						OR ( Jscore>=@MinCredScore AND (JexistAccLen>=@ExistAcctLen 			-- "(" moved 20/09/07
														OR DATEADD(mm,@SettCredMonths,ISNULL(Jdatesettcred,GETDATE()))>=GETDATE()		-- 21/09/07 (Jdatesettcred,'1900-01-01')
														)		-- 21/09/07 (Jdatesettcred,'1900-01-01')
						AND JhighsettStatus<=@maxstatus
						AND ISNULL(Jreferred,' ')!='Y')
						AND ISNULL(JspouseBDWlegal,' ')!='Y' 
						AND ((ISNULL(Jdatechange,GETDATE())>JCustAddDateChange		-- #10354
							AND @ResidenceChangeApp='true'
							--and DATEDIFF(MONTH, JCustAddDateChange, GETDATE()) <= @addresscheckmonths --IP - 01/03/11 - #3247 - Replaced with below
							AND JCustAddDateChange >= DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
							AND Jscore <= @addresscheckscore)
						OR Jscore > @addresscheckscore
						--or DATEDIFF(MONTH, JCustAddDateChange, GETDATE()) > @addresscheckmonths --IP - 01/03/11 - #3247 - Replaced with below
						OR JCustAddDateChange < DATEADD(m,-@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
						OR @ResidenceChangeApp='false')
						-- check for Employment change
						--					and (Jdatechange>(SELECT MAX(ISNULL(e.datechanged,'1900-01-01')) 
						--									FROM #InstantCredit i INNER JOIN employment e on i.Jcustid=e.custid
						--									WHERE e.dateleft is null)
						AND ((ISNULL(Jdatechange,GETDATE())>JEmploymentDateChange		-- #10354
							AND @EmployChangeApp='true'
							--and DATEDIFF(MONTH, JEmploymentDateChange, GETDATE()) <= @addresscheckmonths --IP - 28/02/11 - #3237 - Replaced with below
							AND JEmploymentDateChange >= DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
							AND Jscore <= @employmentcheckscore) 
						OR Jscore > @employmentcheckscore
						--or DATEDIFF(MONTH, JEmploymentDateChange, GETDATE()) > @addresscheckmonths --IP - 28/02/11 - #3237 - Replaced with the below
						OR JEmploymentDateChange < DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
						OR @EmployChangeApp='false'))
									
						--IP - CR983 - 06/03/09 - Instant Credit - SELECT all the Customers that have qualified
						--and have not qualified for Instant Credit.

						--#19353 - CR18831
						--Previously duplicate rows appeared for a custid, therefore deleting 
						--FROM InstantCreditApprovalChecks WHERE already exists in #instantcredit
						--then re-insert FROM #instantcredit
						DELETE FROM InstantCreditApprovalChecks
						WHERE EXISTS(SELECT TOP 1 'a' 
									FROM #instantcredit i
									WHERE i.HCustid = InstantCreditApprovalChecks.HCustid)
						AND PreapprovalDate = @PreapprovalDate

						INSERT INTO InstantCreditApprovalChecks
						SELECT * FROM #instantcredit 

						-- for all accounts		
									
						UPDATE c
						SET InstantCredit='Y'		-- Qualifies for Instant Credit
						FROM #CustomerForCreditUpdate c 
						INNER JOIN #InstantCredit i ON c.custid=i.Hcustid
						WHERE i.InstantCredit='Y'

						-- If customer does not qualify for Instant Credit they also do not qualify for Loan  
						UPDATE #CustomerForCreditUpdate
						SET InstantCredit ='N', loanqualified =0
						WHERE instantcredit ='Y' 
						AND NOT EXISTS (SELECT TOP 1 'a'  
										FROM #InstantCredit i 
										WHERE #CustomerForCreditUpdate.custid=i.Hcustid AND i.InstantCredit='Y')
						--AND  NOT EXISTS (SELECT * FROM Letter l, custacct ca				-- #8565
						--WHERE l.acctno= ca.acctno AND ca.hldorjnt= 'H' AND ca.custid = customer.custid
						--AND l.lettercode = 'LOAN' AND L.dateacctlttr > DATEADD(MONTH,-@monthsqualifyloan, GETDATE() ))
								   
						-- insert into Letter file (use acctno with latest dateacctopen)	CR906 removed
						--		declare @letterdate DATETIME
						--		SET @letterdate=(SELECT GETDATE())
						--		DECLARE @runno SMALLINT
						--		SELECT @runno =ISNULL(MAX(runno),0) FROM interfacecontrol WHERE interface = 'collections'
									
						INSERT INTO Letter (runno,acctno,dateacctlttr,datedue,lettercode,addtovalue,excelgen)		--UAT6
						SELECT DISTINCT 0,a.acctno,@letterdate,GETDATE(),'INSTCR',NULL,null
						FROM #InstantCredit i 
						INNER JOIN custacct ca ON ca.custid=i.hcustid
						INNER JOIN acct a ON ca.acctno=a.acctno AND ca.hldorjnt='H'
						WHERE InstantCredit='Y' AND a.accttype !='C'
						AND a.dateacctopen=(SELECT MAX(dateacctopen) 
											FROM acct a2 
											INNER JOIN custacct ca2 ON a2.acctno=ca2.acctno
											AND ca2.hldorjnt='H' 
											AND ca2.custid=i.hcustid AND a2.accttype !='C') --letters generated should be for HP accounts.
									
					END   -- @piProcess = 'I'				-- CR906

					-- Cash Loans Processing		-- CR906
					IF @piProcess = 'L'				
					BEGIN 
						/* AA updating below for performance reasons.
						Update Customer
						SET LoanQualified='0'		-- ReSET Cash Loan first
						WHERE LoanQualified='1'
						*/
									
						UPDATE #InstantCredit
						SET LoanQualified='Y'		-- Qualifies for Instant Credit	
						FROM #InstantCredit i			-- jec 21/11/07	
						WHERE (HExistingCustomer = 1 AND Hscore>=@MinCredScore 
							OR(HRecentCustomer = 1 AND Hscore >=@MinCredScoreRecent)
							OR(HNewCustomer = 1 AND Hscore >= @MinCredScoreNew))				
						--or DATEADD(mm,@SettCredMonths,ISNULL(Hdatesettcred,'1900-01-01'))>=GETDATE() --No longer required as Cash Loans Customers now in a group.
						--or DATEADD(mm,@SettCashMonths,ISNULL(Hdatesettcash,'1900-01-01'))>=GETDATE()
						AND (HhighsettStatus<=@maxstatus AND HExistingCustomer = 1
							OR (HhighSettStatus <= @MaxstatusRecentCust AND HRecentCustomer = 1)
							OR (HNewCustomer = 1))
						--and HhighsettStatus!='0'		-- #8532 -- status will be zero if only cancelled a/cs 69427 jec 14/12/07
						AND ISNULL(Hreferred,' ')!='Y'
						AND ISNULL(HspouseBDWlegal,' ')!='Y'
						AND ((HmaxArrearsLevel<=@MaxArrearsLevel AND HExistingCustomer = 1)
							OR(HmaxArrearsLevel <= @RecentCustMaxArrearsLevel AND HRecentCustomer = 1)
							OR(HNewCustomer = 1))
						AND HRFpcAvail>=@LoanRFpcAvail		-- CR906
						AND ISNULL(HRFpcCashLoan,0)<@MaxPctRFavail	
						-- check for Residence change		-- added for EOD 29/08/07
						--and ((Hdatechange>(SELECT MAX(ISNULL(a.datechange,'1900-01-01'))	-- jec 21/11/07
						--				FROM custaddress a 
						--				WHERE a.datemoved is null AND a.addtype='H' AND i.Hcustid=a.custid)	
						AND ((((Hdatechange>HCustAddDateChange			-- dateauth since Address change -- CR1232
							AND HCustAddDateChange>DATEADD(m,-@addresscheckmonths,GETDATE()))	-- Address changed within param period
							OR (HCustAddDateChange<DATEADD(m,-@addresscheckmonths,GETDATE()))	)	-- OR Address changed outside param period				
							AND @ResidenceChangeApp='true')
							OR @ResidenceChangeApp='false'          -- jec 29/08/07
							OR HNewCustomer = 1)	
						-- check for Employment change		-- added for EOD 29/08/07
						--and((Hdatechange>(SELECT MAX(ISNULL(e.datechanged,'1900-01-01'))	-- jec 21/11/07
						--				FROM employment e 
						--				WHERE e.dateleft is null AND i.Hcustid=e.custid)
						AND((((Hdatechange>HEmploymentDateChange		-- dateauth since Employment change -- CR1232
							AND HEmploymentDateChange>DATEADD(m,-@employmentcheckmonths,GETDATE()))	-- Employment changed within param period -- CR1232
							OR(HEmploymentDateChange<DATEADD(m,-@employmentcheckmonths,GETDATE()))	)	-- Employment changed outside param period -- CR1232
							AND @EmployChangeApp='true')
							OR @EmployChangeApp='false'         -- jec 29/08/07
							OR HNewCustomer = 1)	
						AND (Jcustid ='NoJoint'					-- No Joint Customer
							OR (Jcustid !='NoJoint' AND Jscore=0)	-- Joint Customer with no own accounts - 20/09/07
								OR ((JExistingCustomer = 1 AND Jscore>=@MinCredScore
									OR(JRecentCustomer = 1 AND Jscore>=@MinCredScoreRecent)
									OR (JNewCustomer = 1 AND Jscore>=@MinCredScoreNew)) 			-- "(" moved 20/09/07
							--No longer required as Cash Loan Customers now in a group.		
							--or DATEADD(mm,@SettCredMonths,ISNULL(Jdatesettcred,GETDATE()))>=GETDATE()		-- 21/09/07 (Jdatesettcred,'1900-01-01')
							--or DATEADD(mm,@SettCashMonths,ISNULL(Jdatesettcash,GETDATE()))>=GETDATE()
							-- 21/09/07 (Jdatesettcred,'1900-01-01')
									AND (JhighsettStatus<=@maxstatus AND JExistingCustomer = 1
									OR (JhighsettStatus <= @MaxstatusRecentCust AND JRecentCustomer = 1)
									OR (JNewCustomer = 1))
							AND ISNULL(Jreferred,' ')!='Y'
							AND ISNULL(JspouseBDWlegal,' ')!='Y'
							--)  - moved bracket FROM here - previously incorrect 
							--CR1225 joint was only being checked for residence AND emp changes for individual not all customers
							--and (
							--		Jdatechange>(SELECT MAX(ISNULL(a.datechange,'1900-01-01'))	-- jec 21/11/07
							--			FROM custaddress a 
							--			WHERE a.datemoved is null AND a.addtype='H' AND i.Jcustid=a.custid)		
							--		and @ResidenceChangeApp='true'
							--	)
							--and (	Jdatechange>(SELECT MAX(ISNULL(e.datechanged,'1900-01-01'))	-- jec 21/11/07
							--			FROM employment e 
							--			WHERE e.dateleft is null AND i.Jcustid=e.custid)
							--		and @EmployChangeApp='true'
							--	)
													
							AND ((((ISNULL(Jdatechange,GETDATE())>JCustAddDateChange			-- #10354 -- dateauth since Address change -- CR1232
							AND JCustAddDateChange>DATEADD(m,-@addresscheckmonths,GETDATE())  )		-- #10354 -- Address changed within param period
							OR (JCustAddDateChange<DATEADD(m,-@addresscheckmonths,GETDATE()))	)	-- #10354 -- OR Address changed outside param period		
							AND @ResidenceChangeApp='true')
							OR @ResidenceChangeApp='false'
							OR JNewCustomer = 1)
							AND((ISNULL(Jdatechange,GETDATE())>JEmploymentDateChange		-- #10354 -- dateauth since Employment change -- CR1232
							AND JEmploymentDateChange>DATEADD(m,-@employmentcheckmonths,GETDATE())	-- Address changed within param period -- CR1232
							AND @EmployChangeApp='true')
							OR @EmployChangeApp='false'
							OR JNewCustomer = 1)	-- CR1232
							)	-- CR1232 - moved bracket  
						)
											
						-- For staff accounts - qualify without checking.	
						OR Staff='Y'		-- #8463
											
						-- --#8463 removed  - For staff accounts - qualify without checking.		CR1232 move code
						--or EXISTS (SELECT * FROM acct a              
						--	  INNER JOIN custacct ca ON ca.acctno = a.acctno
						--	  INNER JOIN customer c on ca.custid = c.custid				--IP - 13/10/11 - #8439 - CR1232
						--	  --INNER JOIN #InstantCredit i on ca.custid=i.Hcustid
						--	  WHERE currstatus = '9'
						--	  AND hldorjnt = 'H'				  
						--	  AND ca.custid=i.Hcustid		-- correct code
						--	  AND c.availablespend >0 )		--IP - 13/10/11 - #8439 - CR1232 
											
						-- ReSET Unblocked flag			#8596 		
						UPDATE #CustomerForCreditUpdate 
						SET CashLoanBlocked=''
						WHERE ISNULL(CashLoanBlocked,'')='U'
											
						UPDATE c
						SET LoanQualified=1		-- Qualifies for Cash Loan
						FROM #CustomerForCreditUpdate c 
						INNER JOIN #InstantCredit i ON c.custid = i.Hcustid
						WHERE ((i.LoanQualified='Y' AND Rfcreditlimit>0)		-- must have a credit limit	#8766
						OR Staff='Y')		--#8463	
						AND creditblocked = 0		--#17530 - Cannot be credit blocked

						-- UnSET loan qualification if customer has not qualified in this run AND date of last loan letter outside qualifcation period
						UPDATE #CustomerForCreditUpdate
						SET LoanQualified =0 
						WHERE LoanQualified =1 
						AND NOT EXISTS (SELECT TOP 1 'a' FROM #InstantCredit i WHERE #CustomerForCreditUpdate.custid=i.Hcustid AND ((i.LoanQualified='Y' AND Rfcreditlimit>0) OR Staff='Y'))
						OR creditblocked = 1			--#8463
						-- #8565			--AND  NOT EXISTS (SELECT * FROM Letter l, custacct ca 
						--WHERE l.acctno= ca.acctno AND ca.hldorjnt= 'H' AND ca.custid = customer.custid
						--AND l.lettercode = 'LOAN' 
						--AND L.dateacctlttr > DATEADD(MONTH,-@monthsqualifyloan, GETDATE() )) 
								   
						-- insert into Letter file (use acctno with latest dateacctopen)	CR906 removed
						--declare @letterdate DATETIME
						--SET @letterdate=(SELECT GETDATE())

						UPDATE c 
						SET CashLoanNew = CASE
							WHEN i.HNewCustomer = 1 AND i.Staff = 'N'
							THEN 1
							ELSE 0
							END,
						CashLoanRecent = CASE
							WHEN i.HRecentCustomer = 1 AND i.Staff = 'N'
							THEN 1
							ELSE 0
							END,
						CashLoanExisting = CASE WHEN i.HExistingCustomer = 1 AND i.Staff = 'N'
							THEN 1
							ELSE 0
							END,
						CashLoanStaff = CASE WHEN i.Staff = 'Y'
							THEN 1
							ELSE 0
							END
						FROM #InstantCredit i
						INNER JOIN #CustomerForCreditUpdate  c ON c.Custid = i.HCustId 
						WHERE c.LoanQualified = 1

						INSERT INTO Letter (runno,acctno,dateacctlttr,datedue,lettercode,addtovalue,excelgen)	--UAT6
						SELECT DISTINCT 0,a.acctno,@letterdate,GETDATE(),'LoanE',NULL,null
						FROM #InstantCredit i 
						INNER JOIN custacct ca ON ca.custid=i.hcustid
						INNER JOIN acct a ON ca.acctno=a.acctno AND ca.hldorjnt='H' AND a.accttype !='C'
						WHERE LoanQualified='Y'
						AND a.dateacctopen=(SELECT MAX(dateacctopen) 
											FROM acct a2 
											INNER JOIN custacct ca2 ON a2.acctno=ca2.acctno
												AND ca2.hldorjnt='H' AND ca2.custid=i.hcustid AND a2.accttype !='C')
						AND NOT EXISTS(SELECT TOP 1 'a' FROM CashLoan cl WHERE i.HCustid=cl.custid AND LoanStatus='D')				-- #8565 - customer has not taken Cash Loan 
						AND i.HExistingCustomer = 1
						--and not exists (SELECT 1 FROM lineitem							-- #8565
						--				INNER JOIN StockInfo s on lineitem.ItemId=s.ID   
						--			   INNER JOIN acct a on a.acctno = lineitem.acctno AND a.currstatus != 'S' AND a.accttype != 'C'		-- correction jec 29/07/11 a.currstatus != 'C'
						--			   WHERE IUPC = 'LOAN'					-- RI itemno = 'LOAN'  
						--			   AND lineitem.acctno in (SELECT acctno   
						--				   FROM custacct ca3   
						--				   WHERE ca3.custid = ca.custid)  
						--			   )
						--and not exists (SELECT * FROM letter l, custacct ca				
						--				WHERE l.acctno = ca.acctno AND ca.hldorjnt = 'H' AND ca.custid = i.hcustid							--IP 26/10/11 - #3904 - CR1232 --Exclude customers that are still within qualifying period
						--				and l.lettercode = 'LOAN')
						--				and l.dateacctlttr >DATEADD(MONTH,-@monthsqualifyloan, GETDATE()))		-- #8565


						INSERT INTO Letter (runno,acctno,dateacctlttr,datedue,lettercode,addtovalue,excelgen)	--UAT6
						SELECT DISTINCT 0,a.acctno,@letterdate,GETDATE(),'LoanR',NULL,null
						FROM #InstantCredit i 
						INNER JOIN custacct ca ON ca.custid=i.hcustid
						INNER JOIN acct a ON ca.acctno=a.acctno AND ca.hldorjnt='H' AND a.accttype !='C'
						WHERE LoanQualified='Y'
						AND a.dateacctopen=(SELECT MAX(dateacctopen) 
										FROM acct a2 
										INNER JOIN custacct ca2 ON a2.acctno=ca2.acctno
										AND ca2.hldorjnt='H' AND ca2.custid=i.hcustid AND a2.accttype !='C')
						AND NOT EXISTS(SELECT TOP 1 'a'  FROM CashLoan cl WHERE i.HCustid=cl.custid AND LoanStatus='D')				-- #8565 - customer has not taken Cash Loan 
						AND i.HRecentCustomer = 1

						--IP - 26/10/11 - #3905 - CR1232 - Send a settled Cash Loan letter
						INSERT INTO Letter (runno,acctno,dateacctlttr,datedue,lettercode,addtovalue,excelgen)
						SELECT DISTINCT 0,s.acctno , @letterdate, GETDATE(), 'LoanS', NULL, null
						FROM #InstantCredit i 
						INNER JOIN custacct ca ON ca.custid = i.hcustid
						INNER JOIN CashLoan cl ON ca.acctno = cl.acctno
						INNER JOIN acct a1 ON cl.acctno = a1.acctno
						INNER JOIN status s ON ca.acctno = s.acctno
						WHERE LoanQualified = 'Y'
						AND cl.loanstatus = 'D'
						AND a1.currstatus = 'S'
						--Most recent Cash Loan settled more than x months
						AND s.datestatchge = (SELECT MAX(datestatchge)
											FROM status s2 
											INNER JOIN custacct ca2 ON s2.acctno = ca2.acctno AND ca2.hldorjnt = 'H'
											INNER JOIN CashLoan cl ON ca2.acctno = cl.acctno AND cl.LoanStatus = 'D'
											WHERE s2.statuscode = 'S'
											AND ca2.custid = ca.custid)
						AND s.datestatchge < DATEADD(MONTH, -@CashLoanLetterPrevSettMths, GETDATE())						
						AND (
						--Letter does not exist
						NOT EXISTS(SELECT TOP 1 'a' FROM letter l	
									INNER JOIN custacct ca3 ON l.acctno = ca3.acctno AND ca3.hldorjnt = 'H'	
									WHERE l.lettercode = 'LoanS'
									AND ca3.custid = i.hcustid)
						--Letter exists 
						OR (exists (SELECT TOP 1 'a' FROM letter l2 INNER JOIN custacct ca4 on l2.acctno = ca4.acctno AND ca4.hldorjnt = 'H'
									WHERE l2.lettercode = 'LoanS'
									and ca4.custid = i.hcustid 
									)
						--and has had a settled cash loan since the last letter
						AND EXISTS(SELECT TOP 1 'a' 
									FROM CashLoan cl3 
									INNER JOIN status s3 ON cl3.acctno = s3.acctno AND s3.statuscode = 'S'
									WHERE s3.datestatchge = (SELECT MAX(datestatchge)
															FROM status s4 
															INNER JOIN custacct ca5 ON s4.acctno = ca5.acctno AND ca5.hldorjnt = 'H'
															WHERE s4.statuscode = 'S'
															AND ca5.custid = cl3.custid
															AND ca5.custid = i.hcustid)
									--and datesettled > letter date
									AND s3.datestatchge > (SELECT MAX(dateacctlttr) FROM letter l3
															INNER JOIN custacct ca6 ON l3.acctno = ca6.acctno AND ca6.hldorjnt = 'H'
															WHERE l3.lettercode = 'LoanS'
															AND ca6.custid = cl3.custid
															AND ca6.custid = i.hcustid)
								)	 
							)
							)
							AND NOT EXISTS (SELECT TOP 1 'a' 
											FROM acct a2 
											INNER JOIN custacct ca7 ON a2.acctno = ca7.acctno AND ca7.hldorjnt = 'H'		--IP - 14/11/11 - #8622 - AND the customer does not have a current Cash Loan
											INNER JOIN CashLoan cl4 ON a2.acctno = cl4.acctno
											WHERE cl4.LoanStatus = 'D'
											AND a2.currstatus!='S'
											--and a2.dateacctopen > s.datestatchge			--IP - 15/11/11 - #8624
											AND ca7.custid = i.Hcustid)

						--IP - 27/10/11 - #3906 - CR1232 - send a percentage paid Cash Loan letter
						INSERT INTO Letter (runno,acctno,dateacctlttr,datedue,lettercode,addtovalue,excelgen)
						SELECT DISTINCT 0, a.acctno, @letterdate, GETDATE(), 'LoanP', NULL, null
						FROM #CustomerForCreditUpdate  c
						INNER JOIN custacct ca ON c.custid = ca.custid AND ca.hldorjnt = 'H'
						INNER JOIN acct a ON ca.acctno = a.acctno
						INNER JOIN cashloan cl ON a.acctno = cl.acctno
						WHERE c.LoanQualified = 1
						AND a.currstatus!='S'
						AND a.paidpcent >= @CashLoanPercentagePaid
						AND NOT EXISTS(SELECT TOP 1 'a' FROM letter l
										WHERE l.acctno = a.acctno
										AND l.lettercode = 'LoanP')
					END   -- @piProcess = 'L'				-- CR906
				END	  -- ISNULL(@piCustomerID,' ')=' '
				ELSE --single customer id
				BEGIN
					-- OR update the returned flag 
					-- Instant credit Processing
					IF @piProcess = 'I'				-- CR906
					BEGIN 
						--cr1225 if joint turned off SET no joint customer
						IF @jointqualification = 'false'
							UPDATE #instantcredit
							SET Jcustid ='NoJoint'	

						IF EXISTS(SELECT TOP 1 'a' 
								FROM #InstantCredit i			-- jec 21/11/07 i
								WHERE Hscore>=@MinCredScore 
								AND (HexistAccLen>=@ExistAcctLen OR DATEADD(mm,@SettCredMonths,ISNULL(Hdatesettcred,'1900-01-01'))>=GETDATE()
									)
								AND HhighsettStatus<=@maxstatus
								--and HhighsettStatus!='0'		-- #8532 -- status will be zero if only cancelled a/cs 69427 jec 14/12/07
								AND ISNULL(Hreferred,' ')!='Y'
								AND ISNULL(HspouseBDWlegal,' ')!='Y'
								AND ((Hdatechange>HCustAddDateChange --CR1225
								AND @ResidenceChangeApp='true'
									--and DATEDIFF(MONTH, HCustAddDateChange, GETDATE()) <= @addresscheckmonths --IP - 01/03/11 - #3247 Replaced with below
									AND HCustAddDateChange >= DATEADD(m,-@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
									AND Hscore <= @addresscheckscore)
									OR Hscore > @addresscheckscore
									--or DATEDIFF(MONTH, HCustAddDateChange, GETDATE()) > @addresscheckmonths --IP - 01/03/11 - #3247 Replaced with below
									OR HCustAddDateChange < DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
									OR @ResidenceChangeApp='false')	-- jec 29/08/07
								AND((Hdatechange>HEmploymentDateChange
									AND @EmployChangeApp='true'
									--and DATEDIFF(MONTH, HEmploymentDateChange, GETDATE()) <= @addresscheckmonths --IP - 28/02/11 - #3237 Replaced with below
									AND HEmploymentDateChange >= DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237 
									AND Hscore <= @employmentcheckscore) 
									OR Hscore > @employmentcheckscore
									--or DATEDIFF(MONTH, HEmploymentDateChange, GETDATE()) > @addresscheckmonths --ip - 28/02/11 - #3237 - Replaced with below
									OR HEmploymentDateChange < DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
									OR @EmployChangeApp='false')	-- jec 29/08/07
									AND (Jcustid ='NoJoint'					-- No Joint Customer
									OR (Jcustid !='NoJoint' AND Jscore=0)	-- Joint Customer with no own accounts - 20/09/07
									OR (  Jscore>=@MinCredScore AND (JexistAccLen>=@ExistAcctLen		-- "(" moved 20/09/07
									OR DATEADD(mm,@SettCredMonths,ISNULL(Jdatesettcred,GETDATE()))>=GETDATE()		-- 21/09/07 (Jdatesettcred,'1900-01-01')
																	)		-- 21/09/07 (Jdatesettcred,'1900-01-01')
									AND JhighsettStatus<=@maxstatus
									AND ISNULL(Jreferred,' ')!='Y')
									AND ISNULL(JspouseBDWlegal,' ')!='Y'
								-- check for Residence change - allow for 2 days grace
								AND ((ISNULL(Jdatechange,GETDATE())>JCustAddDateChange		-- #10354
									AND @ResidenceChangeApp='true'
									--and DATEDIFF(MONTH, JCustAddDateChange, GETDATE()) <= @addresscheckmonths --IP - 01/03/11 - #3247 - Replaced with below
									AND JCustAddDateChange >= DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
									AND Jscore <= @addresscheckscore)
									OR Jscore > @addresscheckscore
									--or DATEDIFF(MONTH, JCustAddDateChange, GETDATE()) > @addresscheckmonths --IP - 01/03/11 - #3247 - Replaced with below
									OR JCustAddDateChange < DATEADD(m,-@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
									OR @ResidenceChangeApp='false')
								-- check for Employment change
								AND ((ISNULL(Jdatechange,GETDATE())>JEmploymentDateChange		-- #10354
									AND @EmployChangeApp='true'
									--and DATEDIFF(MONTH, JEmploymentDateChange, GETDATE()) <= @addresscheckmonths --IP - 28/02/11 - #3237 - Replaced with below
									AND JEmploymentDateChange >= DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
									AND Jscore <= @employmentcheckscore) 
									OR Jscore > @employmentcheckscore
									--or DATEDIFF(MONTH, JEmploymentDateChange, GETDATE()) > @addresscheckmonths --IP - 28/02/11 - #3237 - Replaced with below
									OR JEmploymentDateChange < DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
									OR @EmployChangeApp='false'))
									)
						BEGIN
							SET @poInstantCredit='Y'
							-- but check whether HP account AND if so don't allow
							IF EXISTS ( SELECT value FROM CountryMaintenance WHERE codename LIKE 'HPQualInstantCredit'
									AND value = 'FALSE')
								IF EXISTS (	SELECT TOP 1 'a' FROM acct WHERE acctno= @piAccountNo 	AND accttype !='R')
									SET @poInstantCredit='N'

							-- flag instalplan as Instant Credit
										
							UPDATE Instalplan
							SET InstantCredit=@poInstantCredit
							WHERE acctno=@piAccountNo

							IF  @poInstantCredit='Y'
								INSERT INTO instantcreditflag (custid, acctno, checktype)
								SELECT hcustid, @piAccountNo, 'ARR'
								FROM #instantCredit
								WHERE HmaxArrearsLevel>@MaxArrearsLevel

						END
						ELSE
						BEGIN
							-- flag instalplan as Refused Instant Credit
							UPDATE Instalplan
							SET InstantCredit='R'
							WHERE acctno=@piAccountNo
						END
					END		--  @piProcess = 'I'				-- CR906

				-- Cash Loans Processing		-- CR906
					IF @piProcess = 'L'				
					BEGIN 
						IF EXISTS(SELECT TOP 1 'a' FROM #InstantCredit i			-- jec 21/11/07 i
								--WHERE Hscore>=@MinCredScore 
								--and (HexistAccLen>=@ExistAcctLen
								WHERE ((HExistingCustomer = 1
										OR(HRecentCustomer = 1)
										OR(HNewCustomer = 1))
									-- #8558 07/11/11 					
									--or DATEADD(mm,@SettCredMonths,ISNULL(Hdatesettcred,'1900-01-01'))>=GETDATE() -- no longer required for Cash Loan as Customer must fall into a group.
									--or DATEADD(mm,@SettCashMonths,ISNULL(Hdatesettcash,'1900-01-01'))>=GETDATE()
									)
								--and HhighsettStatus<=@maxstatus				-- #3900
								--and HhighsettStatus!='0'		-- #8532 -- status will be zero if only cancelled a/cs 69427 jec 14/12/07
								AND ISNULL(Hreferred,' ')!='Y'
								AND ISNULL(HspouseBDWlegal,' ')!='Y'			
								--and HmaxArrearsLevel<=@MaxArrearsLevel			-- #3900
								--and HRFpcAvail>=@LoanRFpcAvail		-- #8558 09/11/11 
								AND ISNULL(HRFpcCashLoan,0)<@MaxPctRFavail		-- Cash Loan percent < limit - CR1232
								AND (HAcctType = @AccTypeToQualify		-- Account type to Qualify - CR1232
									OR HAcctType IN ('Bo','HP','RF') AND @AccTypeToQualify = 'Both')

								--and ((Hdatechange>(SELECT MAX(ISNULL(a.datechange,'1900-01-01'))	-- jec 21/11/07
								--				FROM custaddress a 
								--				WHERE a.datemoved is null AND a.addtype='H' AND i.Hcustid=a.custid)				
								--		and @ResidenceChangeApp='true')
								AND ((((Hdatechange>HCustAddDateChange			-- dateauth since Address change -- CR1232
									AND HCustAddDateChange>DATEADD(m,-@addresscheckmonths,GETDATE()))	-- Address changed within param period
									OR (HCustAddDateChange<DATEADD(m,-@addresscheckmonths,GETDATE()))	)	-- OR Address changed outside param period				
									AND @ResidenceChangeApp='true')
									OR @ReferralResidence='true'		-- #8509 jec 31/10/11 
									OR @ResidenceChangeApp='false'      -- jec 29/08/07
									OR HNewCustomer = 1)	

								-- check for Employment change
								--and((Hdatechange>(SELECT MAX(ISNULL(e.datechanged,'1900-01-01'))	-- jec 21/11/07
								--				FROM employment e 
								--				WHERE e.dateleft is null AND i.Hcustid=e.custid)
								AND((((Hdatechange>HEmploymentDateChange		-- dateauth since Employment change -- CR1232
									AND HEmploymentDateChange>DATEADD(m,-@employmentcheckmonths,GETDATE()))	-- Employment changed within param period -- CR1232
									OR(HEmploymentDateChange<DATEADD(m,-@employmentcheckmonths,GETDATE()))	)	-- Employment changed outside param period -- CR1232
									AND @EmployChangeApp='true')
									OR @ReferralEmployment='true'		-- #8509 jec 31/10/11
									OR @EmployChangeApp='false'         -- jec 29/08/07
									OR HNewCustomer = 1)	
								AND (Jcustid ='NoJoint'					-- No Joint Customer
								OR (Jcustid !='NoJoint' AND Jscore=0)	-- Joint Customer with no own accounts - 20/09/07
								OR ((JExistingCustomer = 1 AND Jscore >= @MinCredScore
								OR(JRecentCustomer = 1 AND Jscore >= @MinCredScoreRecent)
								OR (JNewCustomer = 1 AND Jscore >= @MinCredScoreNew)) 				-- "(" moved 20/09/07
								--or DATEADD(mm,@SettCredMonths,ISNULL(Jdatesettcred,GETDATE()))>=GETDATE()		-- 21/09/07 (Jdatesettcred,'1900-01-01') --No longer required as Customers now in a group.
								--or DATEADD(mm,@SettCashMonths,ISNULL(Jdatesettcash,GETDATE()))>=GETDATE()
								-- 21/09/07 (Jdatesettcred,'1900-01-01')
								AND (JhighsettStatus<=@maxstatus AND JExistingCustomer = 1
									OR (JhighsettStatus <= @MaxstatusRecentCust AND JRecentCustomer = 1)
									OR (JNewCustomer = 1))
								AND ISNULL(Jreferred,' ')!='Y')
								AND ISNULL(JspouseBDWlegal,' ')!='Y'

								--and (Jdatechange>(SELECT MAX(ISNULL(a.datechange,'1900-01-01'))	-- jec 21/11/07
								--			FROM custaddress a 
								--			WHERE a.datemoved is null AND a.addtype='H' AND i.Jcustid=a.custid)
								AND ((((ISNULL(Jdatechange,GETDATE())>JCustAddDateChange			-- #10354 -- dateauth since Address change -- CR1232
									AND JCustAddDateChange>DATEADD(m,-@addresscheckmonths,GETDATE()) )	-- #10354 -- Address changed within param period
									OR (JCustAddDateChange<DATEADD(m,-@addresscheckmonths,GETDATE()))	)	-- #10354 -- OR Address changed outside param period		
									AND @ResidenceChangeApp='true')
									OR @ResidenceChangeApp='false'
									OR JNewCustomer = 1)
								-- check for Employment change
								--					and (Jdatechange>(SELECT MAX(ISNULL(e.datechanged,'1900-01-01')) 
								--									FROM #InstantCredit i INNER JOIN employment e on i.Jcustid=e.custid
								--									WHERE e.dateleft is null)
								--and (Jdatechange>(SELECT MAX(ISNULL(e.datechanged,'1900-01-01'))	-- jec 21/11/07
								--			FROM employment e 
								--			WHERE e.dateleft is null AND i.Jcustid=e.custid)
								AND((ISNULL(Jdatechange,GETDATE())>JEmploymentDateChange		-- #10354 -- dateauth since Employment change -- CR1232
									AND JEmploymentDateChange>DATEADD(m,-@employmentcheckmonths,GETDATE())	-- Address changed within param period -- CR1232
									AND @EmployChangeApp='true')
									OR @EmployChangeApp='false'             -- CR1232
									OR JNewCustomer = 1)	
								) )
							-- must have a credit limit   #8756
							AND (SELECT rfcreditlimit FROM #CustomerForCreditUpdate  WHERE custid=@piCustomerID)>0
							AND (SELECT creditblocked FROM #CustomerForCreditUpdate  WHERE custid=@piCustomerID) = 0 --#17530 - Customer cannot have credit blocked
							-- For staff accounts - qualify without checking.	CR1232 move code
							OR EXISTS (SELECT TOP 1 'a' FROM #instantcredit WHERE Staff='Y'
							AND (SELECT creditblocked FROM #CustomerForCreditUpdate  WHERE custid=@piCustomerID) = 0) --#17530 - Customer cannot have credit blocked		-- #8463
						BEGIN
							SET @poLoanQualified='Y'
											
							IF NOT EXISTS (SELECT TOP 1 'a' FROM #instantcredit WHERE Staff='Y' AND HCustid=@piCustomerID)		-- Not Staff
							BEGIN
							-- Referral messages       #3900
								IF EXISTS(SELECT TOP 1 'a' FROM #instantcredit WHERE (HmaxArrearsLevel>@MaxArrearsLevel AND HExistingCustomer = 1
											OR HmaxArrearsLevel > @RecentCustMaxArrearsLevel AND HRecentCustomer = 1))
								BEGIN
									IF @ReferralArrears=1				-- ReferralMsg turned on
										UPDATE #RefMessages SET Msg1='Y'
									ELSE
										SET @poLoanQualified='N'
								END
											
								IF EXISTS(SELECT TOP 1 'a' FROM #instantcredit WHERE HCustid=@piCustomerID AND HScore<@MinCredScore AND HExistingCustomer = 1
									OR EXISTS(SELECT TOP 1 'a' FROM #instantcredit WHERE HCustid=@piCustomerID AND HScore<@MinCredScoreRecent AND HRecentCustomer = 1)
									OR EXISTS (SELECT TOP 1 'a' FROM #instantcredit WHERE HCustid=@piCustomerID AND HScore<@MinCredScoreNew AND HNewCustomer = 1))	-- #8558 
								BEGIN
									IF @ReferralRescored=1
									BEGIN
										UPDATE #RefMessages SET Msg2='Y'
									END
									ELSE
										SET @poLoanQualified='N'
								END	
								IF EXISTS(SELECT TOP 1 'a' FROM #instantcredit WHERE HCustid=@piCustomerID AND ((HhighsettStatus>@maxstatus AND HExistingCustomer = 1)
									OR (HhighSettStatus >@MaxstatusRecentCust AND HRecentCustomer = 1)) AND Staff!='Y')
								BEGIN
									IF @ReferralStatus=1				-- ReferralMsg turned on
									BEGIN
									--UPDATE #RefMessages SET Msg3='Y'
										INSERT INTO #Refstatus					
										SELECT s.acctno 
										FROM #instantcredit i 
										INNER JOIN custacct ca ON i.HCustid=ca.custid 
										INNER JOIN status s ON ca.acctno=s.acctno
										WHERE statuscode>@maxstatus 
										AND datestatchge>=DATEADD(m,@HighStatusTimeFrame * -1,GETDATE())		-- #3900
										AND statuscode NOT IN('U','O','S')
										IF @@ROWCOUNT>0									-- #8546
											UPDATE #RefMessages SET Msg3='Y'
									END
									ELSE
										SET @poLoanQualified='N'
								END
								IF EXISTS (SELECT TOP 1 'a' FROM #instantcredit WHERE (((Hdatechange<HCustAddDateChange			-- dateauth since Address change -- CR1232
									AND HCustAddDateChange>DATEADD(m,-@addresscheckmonths,GETDATE()))	-- Address changed within param period
									--or (HCustAddDateChange>DATEADD(m,-@addresscheckmonths,GETDATE()))	-- OR Address changed outside param period	
									)				
									AND @ResidenceChangeApp='true'
									AND HNewCustomer != 1
									))
								BEGIN
									IF @ReferralResidence=1				-- ReferralMsg turned on
										UPDATE #RefMessages SET Msg4='Y'
									ELSE
										SET @poLoanQualified='N'
								END
											
								IF EXISTS (SELECT TOP 1 'a' FROM #instantcredit WHERE (((Hdatechange<HEmploymentDateChange		-- dateauth since Employment change -- CR1232
									and HEmploymentDateChange>DATEADD(m,-@employmentcheckmonths,GETDATE()))	-- Employment changed within param period -- CR1232
									--or(HEmploymentDateChange>DATEADD(m,-@employmentcheckmonths,GETDATE()))	-- Employment changed outside param period -- CR1232
									)	
									and @EmployChangeApp='true'
									and HNewCustomer != 1
									))
								BEGIN
									IF @ReferralEmployment=1				-- ReferralMsg turned on
										UPDATE #RefMessages SET Msg5='Y'
									ELSE
										SET @poLoanQualified='N'
								END
								IF EXISTS (SELECT TOP 1 'a' FROM #instantcredit i INNER JOIN #CustomerForCreditUpdate  c ON HCustid=c.custid WHERE AvailableSpend>@MinLoanAmount
									AND (RFCreditLimit=0 OR AvailableSpend/RFcreditlimit*100 < @LoanRFpcAvail)		-- #8756
									OR AvailableSpend<@MinLoanAmount)			-- #8780
								BEGIN
									IF @ReferralPercentage=1				-- ReferralMsg turned on
										UPDATE #RefMessages SET Msg6='Y'
									ELSE
										SET @poLoanQualified='N'
								END
							END
							-- return referral data 				
							SELECT * FROM #ArrearsLevel WHERE ArrearsLevel>@MaxArrearsLevel
							SELECT * FROM #RefStatus
							SELECT * FROM #RefMessages
							-- return Existing Cash Loans value		#8586 
							IF EXISTS(SELECT TOP 1 'a' FROM #Cashloan)
								SELECT ISNULL(OutstBalLoan,0) as CashLoanPC FROM #Cashloan WHERE custid=@piCustomerID
							ELSE
								SELECT 0 as CashLoanPC

							IF(@poLoanQualified = 'Y')
							BEGIN
								UPDATE c  
								SET CashLoanNew = CASE
								WHEN i.HNewCustomer = 1 AND i.Staff = 'N'
								THEN 1
								ELSE 0
								END,
								CashLoanRecent = CASE
								WHEN i.HRecentCustomer = 1 AND i.Staff = 'N'
								THEN 1
								ELSE 0
								END,
								CashLoanExisting = CASE WHEN i.HExistingCustomer = 1 AND i.Staff = 'N'
								THEN 1
								ELSE 0
								END,
								CashLoanStaff = CASE WHEN i.Staff = 'Y'
								THEN 1
								ELSE 0
								END
								FROM #InstantCredit i
								INNER JOIN #CustomerForCreditUpdate   c on c.Custid = i.HCustId 
								WHERE i.HCustId = @piCustomerID
							END				
						END
					END		--  @piProcess = 'L'				-- CR906
				END
			END		-- End of @ExistInstCredit!='Y'
			ELSE
			BEGIN
				-- Instant Credit already granted(or refused) - SET out parm to G so that pop-up message is not displayed (New/Revised Account screen)
				SET @poInstantCredit='G'
				return
			END
		END		-- End of Checking

		IF @piProcess = 'I' AND @piCustomerID!=' ' AND @piCustomerID!='??'
		BEGIN 
			SELECT * FROM #instantcredit

			INSERT INTO dbo.ICAnalysisReport
			( custid ,JCustid ,acctno , accttype ,applicationdate,dateagrmt ,
			points ,InstantCredit ,CreditHistory ,AccountStatus ,
			CurrentArrears ,HPAccounts ,CreditScore ,ReferredAccount ,
			LegalAction ,ResidenceChanged ,EmploymentChanged ,JointHolder
			)
			SELECT  Hcustid, -- custid - VARCHAR(20)
			JCustid , -- JCustid - VARCHAR(20)
			a.acctno , -- acctno - VARCHAR(12)
			a.accttype , -- accttype - CHAR(1)
			@datequalification, --application date
			ag.dateagrmt , -- dateagrmt - DATETIME
			Hscore , -- points - int
			@poInstantCredit , -- InstantCredit - CHAR(1)
			'N' , -- CreditHistory - CHAR(1)
			'N' , -- AccountStatus - CHAR(1)
			'N' , -- CurrentArrears - CHAR(1)
			'N' , -- HPAccounts - CHAR(1)
			'N' , -- CreditScore - CHAR(1)
			'N' , -- ReferredAccount - CHAR(1)
			'N' , -- LegalAction - CHAR(1)
			'N' , -- ResidenceChanged - CHAR(1)
			'N' , -- EmploymentChanged - CHAR(1)
			'N'  -- JointHolder - CHAR(1)
			FROM #instantcredit i
			CROSS JOIN acct a  
			INNER JOIN agreement ag ON a.acctno = ag.acctno
			WHERE a.acctno = @piAccountNo 

			UPDATE r
			SET CreditHistory = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo
			AND r.applicationdate = @datequalification
			AND (HexistAccLen>=@ExistAcctLen 					
			OR DATEADD(mm,@SettCredMonths,ISNULL(Hdatesettcred,'1900-01-01'))>=GETDATE())

			UPDATE r
			SET AccountStatus = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo
			AND r.applicationdate = @datequalification
			AND HhighsettStatus<=@maxstatus

			UPDATE r
			SET CurrentArrears = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo
			AND r.applicationdate = @datequalification
			AND HmaxArrearsLevel<=@MaxArrearsLevel

			UPDATE r
			SET HPAccounts = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo
			AND r.applicationdate = @datequalification
			AND (
			(SELECT value FROM CountryMaintenance WHERE codename LIKE 'HPQualInstantCredit') = 'TRUE'
			OR r.accttype = 'R'
			)

			UPDATE r
			SET CreditScore = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo
			AND Hscore>=@MinCredScore 			

			UPDATE r
			SET ReferredAccount = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo
			AND ISNULL(Hreferred,' ')!='Y'

			UPDATE r
			SET LegalAction = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid 	AND 
			r.acctno = @piAccountNo
			AND ISNULL(HspouseBDWlegal,' ')!='Y'	

			UPDATE r
			SET ResidenceChanged = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid 	AND 
			r.acctno = @piAccountNo
			AND ((Hdatechange>HCustAddDateChange 
					AND @ResidenceChangeApp='true'
					AND HCustAddDateChange >= DATEADD(m,-@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
					AND Hscore <= @addresscheckscore
					)
				OR Hscore > @addresscheckscore
				OR HCustAddDateChange < DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
				OR @ResidenceChangeApp='false'
				)

			UPDATE r
			SET EmploymentChanged = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid 	AND 
			r.acctno = @piAccountNo
			AND ((
					Hdatechange>HEmploymentDateChange
					AND @EmployChangeApp='true'
					AND HEmploymentDateChange >= DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237 
					AND Hscore <= @employmentcheckscore
					) 
				OR Hscore > @employmentcheckscore
				OR HEmploymentDateChange < DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
				OR @EmployChangeApp='false'
				)

			UPDATE r
			SET JointHolder = 'Y'
			FROM #instantcredit i
			INNER JOIN ICAnalysisReport r ON i.hcustid = r.custid 
			WHERE --i.hcustid = r.custid AND 
			r.acctno = @piAccountNo		
			AND ( i.Jcustid ='NoJoint'					-- No Joint Customer
				OR ( i.Jcustid !='NoJoint' AND Jscore=0 )	
				OR (Jscore>=@MinCredScore AND (JexistAccLen>=@ExistAcctLen	OR DATEADD(mm,@SettCredMonths,ISNULL(Jdatesettcred,GETDATE()))>=GETDATE())		
				AND JhighsettStatus<=@maxstatus
				AND ISNULL(Jreferred,' ')!='Y'
				AND ISNULL(JspouseBDWlegal,' ')!='Y'
				AND (
					(ISNULL(Jdatechange,GETDATE())>JCustAddDateChange		-- #10354
						AND @ResidenceChangeApp='true'
						AND JCustAddDateChange >= DATEADD(m, -@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
						AND Jscore <= @addresscheckscore
					)	
					OR Jscore > @addresscheckscore
					OR JCustAddDateChange < DATEADD(m,-@addresscheckmonths, GETDATE()) --IP - 01/03/11 - #3247
					OR @ResidenceChangeApp='false'
					)
												
				AND (
					(ISNULL(Jdatechange,GETDATE())>JEmploymentDateChange		-- #10354
					AND @EmployChangeApp='true'
					AND JEmploymentDateChange >= DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
					AND Jscore <= @employmentcheckscore
					) 
					OR Jscore > @employmentcheckscore
					OR JEmploymentDateChange < DATEADD(m, -@employmentcheckmonths, GETDATE()) --IP - 28/02/11 - #3237
					OR @EmployChangeApp='false'
					)
					)
				)

		
		END
		
		-------------- Update Customer details from Temporary to Actual Table if the values are different
		--DECLARE @RowCount INT
		--SELECT @RowCount = COUNT(c.custid) 
		--FROM Customer C
		--INNER JOIN #CustomerForCreditUpdate Tc ON c.custid=Tc.Custid 
		--WHERE c.InstantCredit= tc.InstantCredit 
		--AND c.loanqualified = tc.loanqualified  
		--AND c.CashLoanBlocked = tc.CashLoanBlocked
		--AND c.CashLoanNew=tc.CashLoanNew
		--AND c.CashLoanRecent=tc.CashLoanRecent
		--AND c.CashLoanExisting=tc.CashLoanExisting
		--AND c.CashLoanStaff=tc.CashLoanStaff
		--AND C.Custid = @piCustomerID
		
		----SELECT @rowCount = @@RowCount
		
		--IF @rowCount=0
		--BEGIN
			UPDATE c
			SET c.InstantCredit= tc.InstantCredit 
			,c.loanqualified = tc.loanqualified  
			,c.CashLoanBlocked = tc.CashLoanBlocked
			,c.CashLoanNew=tc.CashLoanNew
			,c.CashLoanRecent=tc.CashLoanRecent
			,c.CashLoanExisting=tc.CashLoanExisting
			,c.CashLoanStaff=tc.CashLoanStaff
			FROM Customer C
			INNER JOIN #CustomerForCreditUpdate Tc ON c.custid=Tc.Custid 
			--WHERE c.custid = @piCustomerID
		--END

		-- Avoid multipal call on instant credit approvl  		
		INSERT INTO  [dbo].[InstantCreditApprovalsCheckGen_Val]  
		VALUES (@piCustomerID, @piAccountNo, @poInstantCredit, @poLoanQualified,GETDATE()) 
							   
		DELETE FROM [dbo].[InstantCreditApprovalsCheckGen_Val] 
		WHERE (piCustomerID = @piCustomerID ) AND DATEDIFF(SECOND,[ApprovlDate],GETDATE()) > 300
	END
	ELSE
	BEGIN
		SELECT TOP 1 @poInstantCredit = poInstantCredit, @poLoanQualified = poLoanQualified 
		FROM [dbo].[InstantCreditApprovalsCheckGen_Val] 
		WHERE piCustomerID = @piCustomerID 
		AND  piAccountNo = @piAccountNo ORDER BY [ApprovlDate] DESC
	END
	--END
	SET @Return = @@ERROR
END
GO


