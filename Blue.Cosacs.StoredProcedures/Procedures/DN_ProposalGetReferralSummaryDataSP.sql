SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (	SELECT	* 
			FROM	dbo.sysobjects 
			WHERE	id = object_id('[dbo].[DN_ProposalGetReferralSummaryDataSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_ProposalGetReferralSummaryDataSP]

GO

CREATE PROCEDURE 	[dbo].[DN_ProposalGetReferralSummaryDataSP]
			@acctno varchar(12),
			@custid varchar(20),
			@accttype varchar(1),
			@dateprop datetime,
			@return int OUTPUT
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalGetReferralSummaryDataSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve referral summary data
-- Author       : ?
-- Date         : ?
--
-- Change Control
-- --------------
-- Date			By				Description
-- ----			--				-----------
-- 15/03/11		IP				Sprint 5.12 - #3316 - CR1245 - Return propnotes to be displayed in Authorise Delivery, Summary tab, beneath the 
--								Underwriter Information tab
-- 12 Jul 20	Amit Vernekar	Populated MmiLimit column value from Customer table to display it on Under Writing (UW) Referal screen.
--------------------------------------------------------------------------------

AS

	SET 	@return = 0			--initialise return code

	DECLARE	@score smallint,			--
			@riskcategory smallint,		
			@creditlimit money,		--	
			@age smallint,			--
			@income money,		--
			@maritalstat varchar(20),	--
			@residentialstat varchar(20),	--
			@dateinaddress datetime,	--
			@dateemployed datetime,	--
			@expenses money,		--
			@occupation varchar(20),	--
			@repaymentpcent decimal,	--
			@termstype varchar(20),	--
			@monthlyinstal money,		--
			@finalinstal money,
			@deposit money,		--
			@agreementtotal money,		--
			@nocurrent smallint,		--
			@nosettled smallint,		--	
			@worstcurrent varchar(1),	--
			@worstsettled varchar(1),	--
			@outstandingbalance money,	--
			@totalcurrentinstalments money,	--
			@systemrecommendation char(1),

			@noinarrears money,		--
			@longestagreement smallint,	--
			@largestagreement money,	--
			@feesandinterest money,	--
			@nocashsettled smallint,		--
			@repaymentpcentcurrent decimal,	--
			@noreturnedcheques smallint,			-- not sure how to find this out
			@valueofarrears money,		--
			@norfaccounts smallint,		--
			
			@monthlyrent money,

			@CurRecent          char(1),
			@SetRecent          char(1),
			@CurHiNow           char(1) ,
			@SetHiNow           char(1) ,
			@CurWeightAvg       FLOAT,
			@SetWeightAvg       FLOAT,
			@SetLargest         char(1),
			@SetLargestSize     char(1) ,
			@NumAppsLst90       SMALLINT,
			@RejLst90           char(1),
			
			@propNotes varchar(1000),		--IP - 15/03/11 - #3316 - CR1245
			@MmiLimit MONEY

	EXECUTE @return = sp_Transact_CustStatus
			        @piCustId           =	 @custid,
			        @piAcctNo           =  @acctno,
			        @poCurNumAcc        = @nocurrent        OUTPUT,
			        @poSetNumAcc        = @nosettled        OUTPUT,
			        @poCurRecent        = @CurRecent        OUTPUT,
			        @poSetRecent        = @SetRecent        OUTPUT,
			        @poCurHiEver        = @worstcurrent        OUTPUT,
			        @poSetHiEver        = @worstsettled        OUTPUT,
			        @poCurHiNow         = @CurHiNow         OUTPUT,
			        @poSetHiNow         = @SetHiNow         OUTPUT,
			        @poCurWeightAvg     = @CurWeightAvg     OUTPUT,
			        @poSetWeightAvg     = @SetWeightAvg     OUTPUT,
			        @poSetLargest       = @SetLargest       OUTPUT,
			        @poSetLargestSize   = @SetLargestSize   OUTPUT,
			        @poTotalInstal      = @totalcurrentinstalments      OUTPUT,
			        @poTotalOutStBal    = @outstandingbalance    OUTPUT,
			        @poNumAppsLst90     = @NumAppsLst90     OUTPUT,
			        @poRejLst90         = @RejLst90         OUTPUT

	IF(@return = 0)
	BEGIN
		SELECT	acctno 
		INTO		#custacct
		FROM		custacct 
		WHERE	custid = @custid 
		AND		hldorjnt = 'H'

		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT 	@residentialstat = C1.codedescript,
				@dateinaddress = CA.datein,
				@monthlyrent = isnull(CA.mthlyrent,0)
		FROM		custaddress CA LEFT OUTER JOIN code C1
		ON		CA.resstatus = C1.code 
		AND		C1.category = 'RS1'
		WHERE 	CA.custid = @custid 
		AND		CA.addtype = 'H'
		AND 		CA.datemoved is null

		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@income = isnull(P.mthlyincome, 0) + isnull(P.addincome, 0) --+ isnull (P.A2MthlyIncome,0) + isnull (P.A2AddIncome,0)
				-  isnull(P.commitments1, 0) - isnull(P.commitments2, 0) - isnull(P.commitments3, 0) - isnull(P.otherpmnts, 0) 
				- isnull(P.additionalexpenditure1, 0) - isnull(P.additionalexpenditure2, 0) - @monthlyrent,
				@maritalstat = C1.codedescript,
				@score = P.points, @systemrecommendation = P.systemrecommendation,
				@propNotes = P.propnotes, --IP - 15/03/11 - #3316 - CR1245
				@expenses = isnull(P.commitments1, 0) + isnull(P.commitments2, 0) + isnull(P.commitments3, 0) + isnull(P.otherpmnts, 0) 
				+ isnull(P.additionalexpenditure1, 0) + isnull(P.additionalexpenditure2, 0) + @monthlyrent
		FROM		proposal P LEFT OUTER JOIN code C1
		ON		P.maritalstat = C1.code
		AND		C1.category = 'MS1' 
		WHERE	P.custid = @custid
		AND		P.acctno = @acctno
		AND		P.dateprop = @dateprop order by P.dateprop desc
	
		SET	@return = @@error
	END
	
	IF(@return = 0)
	BEGIN
		SELECT	@income = @income + isnull(A2MthlyIncome,0) + isnull(A2AddIncome,0)
		FROM	proposal
		WHERE	custid = @custid
		AND		dateprop = @dateprop
		AND		A2Relation	= 'J'
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@creditlimit = RFCreditLimit,
				@age = age
		FROM	customer 
		WHERE	custid = @custid
	
		SET	@return = @@error
	END


	IF(@return = 0)
	BEGIN

		SELECT @MmiLimit = ISNULL(CM.MmiLimit,0)
		FROM	[dbo].[CustomerMmi] AS CM WITH(NOLOCK)
		WHERE	CM.CustId = @CustId
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@dateemployed = E.dateemployed,
				@occupation = C.codedescript
		FROM		employment E LEFT OUTER JOIN code C
		ON		E.worktype = C.code
		AND		C.category = 'WT1'
		WHERE	custid = @custid	
		AND		dateleft is NULL
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		IF(@income > 0)
		BEGIN
			SELECT	@repaymentpcent = (instalamount / @income) * 100 
			FROM		instalplan 
			WHERE	acctno = @acctno
	
			SET	@return = @@error
		END
	END

	IF(@return = 0)
	BEGIN
		SELECT	@termstype = TT.description,
				@monthlyinstal = IP.instalamount,
				@finalinstal = IP.fininstalamt,
				@deposit = AG.deposit,
				@agreementtotal = AG.agrmttotal
		FROM		acct A INNER JOIN agreement AG
		ON		A.acctno = AG.acctno INNER JOIN instalplan IP
		ON		A.acctno = IP.acctno INNER JOIN termstype TT
		ON		A.termstype = TT.termstype
		WHERE	A.acctno = @acctno 	
		
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT 	@valueofarrears = SUM(isnull(a.Arrears,0))
	        	FROM   	#custacct CA INNER JOIN acct A 
		ON		CA.acctno = A.acctno
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT 	@noinarrears = count(A.arrears)
	        	FROM   	#custacct CA INNER JOIN acct A 
		ON		CA.acctno = A.acctno
		WHERE	A.arrears > 0
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@longestagreement = MAX(IP.instalno)
		FROM   	#custacct CA INNER JOIN  instalplan IP
		ON		CA.acctno = IP.acctno
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@largestagreement = MAX(AG.agrmttotal)
		FROM   	#custacct CA INNER JOIN  agreement AG
		ON		CA.acctno = AG.acctno
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@feesandinterest = SUM(isnull(FT.transvalue,0))
		FROM   	#custacct CA INNER JOIN  fintrans FT
		ON		CA.acctno = FT.acctno
		WHERE	FT.datetrans > ( SELECT 	MAX(A.dateacctopen)
						FROM   	#custacct CA INNER JOIN  acct A
						ON		CA.acctno = A.acctno )
		AND		FT.transtypecode in ('FEE', 'INT')
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	@nocashsettled = COUNT(*)
		FROM   	#custacct CA INNER JOIN  acct A
		ON		CA.acctno = A.acctno
		WHERE	A.currstatus = 'S'	
		AND		A.accttype = 'C'
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		IF(@income > 0)
		BEGIN
			SELECT	@repaymentpcentcurrent = (@totalcurrentinstalments / @income) * 100 
	
			SET	@return = @@error
		END
	END

	IF(@return = 0)
	BEGIN
		SELECT	@norfaccounts = COUNT(*)
		FROM   	#custacct CA INNER JOIN  acct A
		ON		CA.acctno = A.acctno
		WHERE	A.accttype = 'R'	
	
		SET	@return = @@error
	END

	IF(@return = 0)
	BEGIN
		SELECT	isnull(@score ,0) as score,
				isnull(@riskcategory ,0) as riskcategory,
				isnull(@creditlimit ,0) as creditlimit,
				isnull(@age,0) as age,
				isnull(@income ,0) as income,
				isnull(@maritalstat,'') as maritalstatus,
				isnull(@residentialstat ,'') as residentialstatus,
				isnull(@dateinaddress ,'1/1/1900') as dateinaddress,
				isnull(@dateemployed ,'1/1/1900') as dateemployed,
				isnull(@expenses ,0) as expenses,
				isnull(@occupation,'') as occupation,
				isnull(@repaymentpcent,0) as repaymentpcent,
				isnull(@termstype ,'') as termstype,
				isnull(@monthlyinstal , 0) as monthlyinstal,
				isnull(@finalinstal , 0) as finalinstal,
				isnull(@deposit ,0) as deposit,
				isnull(@agreementtotal ,0) as agreementtotal,
				isnull(@nocurrent ,0) as nocurrent,
				isnull(@nosettled ,0) as nosettled,
				isnull(@worstcurrent ,'') as worstcurrent,
				isnull(@worstsettled ,'') as worstsettled,
				isnull(@outstandingbalance ,0) as outstbal,
				isnull(@totalcurrentinstalments ,0) as totalcurrentinstalments,
				isnull(@noinarrears ,0) as noinarrears,
				isnull(@longestagreement ,0) as longestagreement,
				isnull(@largestagreement ,0) as largestagreement,
				isnull(@feesandinterest ,0) as feesandinterest,
				isnull(@nocashsettled ,0) as nocashsettled,
				isnull(@repaymentpcentcurrent ,0) as repaymentpcentcurrent,
				isnull(@noreturnedcheques ,0) as noreturnedcheques,
				isnull(@valueofarrears ,0) as valueofarrears,
				isnull(@norfaccounts, 0) as norfaccounts,
				isnull(@systemrecommendation ,'') as sysrecommend,
				isnull(@propNotes,'') as propnotes,	--IP - 15/03/11 - #3316 - CR1245
				isnull(@MmiLimit,0) as MmiLimit
	END

GO



SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

