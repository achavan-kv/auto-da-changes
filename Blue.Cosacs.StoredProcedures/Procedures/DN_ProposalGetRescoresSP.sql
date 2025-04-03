-- Add Equifax score card condition.
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_ProposalGetRescoresSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_ProposalGetRescoresSP]
GO
/****** Object:  StoredProcedure [dbo].[DN_ProposalGetRescoresSP]    Script Date: 8/18/2018 10:30:04 AM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE 	[dbo].[DN_ProposalGetRescoresSP] 
			@scoretype char(1),
			@return int OUTPUT
AS
	SET nocount on
	SET	@return = 0			--initialise return code

	/*we are going to run this several times as .net error-so need to speed this up */
	IF (@scoretype='A' OR @scoretype='B' OR @scoretype='P' OR @scoretype='S')
	BEGIN 

	DECLARE @numbertoRescore varchar(12), 
			@statement sqltext,
			@months varchar(5)
			
	SELECT	@numbertoRescore = ISNULL(value,'100000') 
		FROM	countrymaintenance
		WHERE	codename = 'numaccts'
				
	if @scoretype = 'A' -- applicant
	BEGIN  
		
		SELECT	@months = ISNULL(value,'12') 
		FROM	countrymaintenance
		WHERE	codename = 'eodrescoremonths'
	END 
	ELSE 
	BEGIN
		-- use Behavioural Rescore Parameter
		DELETE FROM customer_rescore
		SELECT	@months = ISNULL(value,'12') 
		FROM	countrymaintenance
		WHERE	codename = 'eodBehaverescoremonths'
		
	END

	DECLARE @curdate SMALLDATETIME
	SET @curdate = GETDATE()

	-- Log 6493246: Updated DATELASTSCORED based on 
	-- agreed N should be 12 months however if customer booked a sale during the 12 months then the flag would be reset and no need to auto rescore.
	-- e.g. Booked 1st Sale Dec 2017 – scored at date of sale, auto rescore should occur in Dec 2018
	--		Booked 2nd sale July 2018 – rescored with new sale,
	--		Dec 2018 no need to rescore but set flag to July 2019 for next rescore.

	SELECT  MAX(dateagrmt) AS dateagrmt, cust.custid  
	INTO	#CollectmaxAgrDate
	FROM	dbo.agreement WITH(NOLOCK)
			INNER JOIN dbo.acct a WITH(NOLOCK)
				ON a.acctno=agreement.acctno and accttype='R'
			INNER JOIN dbo.custacct cust WITH(NOLOCK)
				ON cust.acctno=a.acctno
	GROUP BY cust.custid

	UPDATE	dbo.customer 
	SET		datelastscored = t.dateagrmt 
	FROM	dbo.customer c
			INNER JOIN #CollectmaxAgrDate t 
				ON t.custid = c.custid  
	WHERE	t.dateagrmt > c.datelastscored	 
	

	IF NOT EXISTS (SELECT * FROM dbo.customer_rescore WHERE date_rescore > DATEADD (HOUR, - 1, GETDATE()) OR @scoretype ='B')
	BEGIN
		TRUNCATE TABLE customer_rescore
		 
		set @statement = ' INSERT INTO dbo.customer_rescore (custid,date_rescore) ' +
						 ' SELECT TOP ' + @numbertoRescore + ' c.custid, ' + 
						 '''' + CONVERT(VARCHAR,@curdate) + '''' + 
						 ' FROM		dbo.acct A INNER JOIN ' +
						 ' dbo.proposal P ON A.acctno = P.acctno INNER JOIN ' +
						 ' dbo.customer C ON P.custid = C.custid INNER JOIN ' +
						 ' accttype AT ON AT.genaccttype = A.accttype ' +
						 ' WHERE	DATEADD(MONTH, ' + @months + ', C.datelastscored) < GETDATE() ' +
						 ' AND		AT.accttype = N''R''  ' +
						 ' AND P.propresult = ''A''  ' +    -- only those which were originally accepted 
						 ' AND		C.creditblocked = 0	 ' +	/* exclude customers with blocked credit */
						 ' AND		NOT EXISTS (SELECT 	1 ' +	/* exclude customers whos RF accounts have expired */
						 ' FROM		dbo.custcatcode CC ' +
						 ' WHERE	CC.custid = C.custid ' +
						 ' AND		CC.code = N''REX'' ' +
						 ' AND		CC.datedeleted is null ) ' --+
-- We are going to rescore all accounts on behavioural that qualify						
--						 ' AND      ISNULL(P.scorecard,''A'') =' + '''' + @scoretype + '''' 
		IF @scoretype = 'B'	-- behavioural scoring make sure that not already rescored
		BEGIN
			SET @statement = @statement + 
			' AND NOT EXISTS (SELECT * FROM dbo.BehaveRescoreExclude b WHERE ' +
			' b.custid = c.custid 	AND DATEADD(MONTH, '  +
			+ @months + ', b.datescored )  > GETDATE()  ) ' 
			
		END

		IF @scoretype = 'B'	-- behavioural scoring need to make sure that accounts are available for rescore - choose ones with instalplan finishing within last year
		BEGIN 
			SET @statement = @statement + 
			' AND EXISTS (SELECT * FROM dbo.instalplan i, custacct ca WHERE ca.custid = c.custid AND i.acctno= ca.acctno AND ca.hldorjnt = ''H''
			AND i.datelast > DATEADD(MONTH,-9,GETDATE())) '
						 
		END 
						 
		set @statement = @statement + ' GROUP BY 	C.custid, c.datelastscored' +
										 ' order by c.datelastscored '
		EXECUTE sp_executesql @statement
		--PRINT @statement
		--IF @@ROWCOUNT = 0
			--PRINT @statement
	END
	
	-- now lets insert customers for rescoring where they have gone into arrears or been reposessed
	IF @scoretype = 'B'
	BEGIN
		EXEC BlockandRescoreBehaviouralCredit @rescoredate = @curdate

	-- here we are storing customers that have been brought back that were originally scored using behavioural scoring
	-- if they now don't qualify because account settled then don't want to continually bring them back
		INSERT INTO  dbo.BehaveRescoreExclude ( custid , datescored)
		SELECT custid,date_rescore FROM dbo.customer_rescore
		WHERE date_rescore = @curdate
	END

	-- here getting number to rescore and then removing those which are not in the top n of those to rescore
	-- otherwise performance may be hindered
   
	SELECT	Max (P.acctno) as 'Account No.',
			Max (AT.accttype) as 'AccountType',	
			C.custid as 'CustomerID',
			MAX(P.dateprop) as 'DateProp',
			C.RFCreditLimit,
			C.datelastscored,
			c.ScoringBand
	FROM	dbo.acct A INNER JOIN 
			dbo.proposal P ON A.acctno = P.acctno INNER JOIN
			dbo.customer_rescore d ON P.custid = d.custid INNER JOIN
			dbo.customer C ON C.custid = d.custid INNER JOIN
			dbo.accttype AT ON AT.genaccttype = A.accttype
			/* Could use DATEDIFF(DAY,datelastscored,GETDATE()) > 365 but the following also works for leap years */
	WHERE	GETDATE() > DATEADD(MONTH, CONVERT(INTEGER, @months), C.datelastscored)
	AND		AT.accttype = 'R'
	AND P.propresult = 'A' -- only those which were originally accepted 
	AND		C.creditblocked = 0		/* exclude customers with blocked credit */
	AND		NOT EXISTS (SELECT 	1	/* exclude customers whos RF accounts have expired */
						FROM	dbo.custcatcode CC
						WHERE	CC.custid = C.custid 
						AND		CC.code = 'REX' 
						AND		CC.datedeleted is null)
	GROUP BY C.custid, C.RFCreditLimit, C.datelastscored, C.ScoringBand

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

	
	END
	ELSE
	BEGIN
	EXEC dbo.Equifax_DN_ProposalGetRescoresSP @scoretype,@return	
	END