
-- New Equifax score 

IF OBJECT_ID('dbo.Equifax_DN_ProposalGetRescoresSP') IS NOT NULL
	DROP PROCEDURE dbo.Equifax_DN_ProposalGetRescoresSP
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

Create PROCEDURE 	[dbo].[Equifax_DN_ProposalGetRescoresSP]
			@scoretype char(1),
			@return int OUTPUT
AS
	SET nocount on
	SET	@return = 0			--initialise return code

	/*we are going to run this several times as .net error-so need to speed this up */

	DECLARE @numbertoRescore varchar(12), 
			@statement sqltext,
			@months varchar(5)
			
	SELECT	@numbertoRescore = ISNULL(value,'100000') 
		FROM	countrymaintenance
		WHERE	codename = 'numaccts'
				
	if @scoretype = 'C' -- Equifax applicant
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
	IF NOT EXISTS (select * from customer_rescore where date_rescore > dateadd (hour, - 1, getdate()) OR @scoretype ='D')  --D=Equifax behavaioural rule
	BEGIN
		TRUNCATE TABLE customer_rescore
		 
		set @statement = ' insert into customer_rescore (custid,date_rescore) ' +
						 ' SELECT TOP ' + @numbertoRescore + ' c.custid, ' + 
						 '''' + CONVERT(VARCHAR,@curdate) + '''' + 
						 ' FROM		acct A INNER JOIN ' +
						 ' proposal P ON A.acctno = P.acctno INNER JOIN ' +
						 ' customer C ON P.custid = C.custid INNER JOIN ' +
						 ' accttype AT ON AT.genaccttype = A.accttype ' +
						 ' WHERE	DATEADD(MONTH, ' + @months + ', C.datelastscored) < GETDATE() ' +
						 ' AND		AT.accttype = N''R''  ' +
						 ' AND P.propresult = ''A''  ' +    -- only those which were originally accepted 
						 ' AND		C.creditblocked = 0	 ' +	/* exclude customers with blocked credit */
						 ' AND		NOT EXISTS (SELECT 	1 ' +	/* exclude customers whos RF accounts have expired */
						 ' FROM		custcatcode CC ' +
						 ' WHERE	CC.custid = C.custid ' +
						 ' AND		CC.code = N''REX'' ' +
						 ' AND		CC.datedeleted is null ) ' --+
-- We are going to rescore all accounts on behavioural that qualify						
--						 ' AND      ISNULL(P.scorecard,''A'') =' + '''' + @scoretype + '''' 
		IF @scoretype = 'D'	-- behavioural scoring make sure that not already rescored
		BEGIN
			SET @statement = @statement + 
			' AND NOT EXISTS (SELECT * FROM BehaveRescoreExclude b WHERE ' +
			' b.custid = c.custid 	AND DATEADD(MONTH, '  +
			+ @months + ', b.datescored )  > GETDATE()  ) ' 
			
		END

		IF @scoretype = 'D'	-- behavioural scoring need to make sure that accounts are available for rescore - choose ones with instalplan finishing within last year
		BEGIN 
			SET @statement = @statement + 
			' AND EXISTS (SELECT * FROM instalplan i, custacct ca WHERE ca.custid = c.custid AND i.acctno= ca.acctno AND ca.hldorjnt = ''H''
			AND i.datelast > DATEADD(MONTH,-9,GETDATE())) '
						 
		END 
						 
		set @statement = @statement + ' GROUP BY 	C.custid, c.datelastscored' +
						 ' order by c.datelastscored '
		EXECUTE sp_executesql @statement
		PRINT @statement
		IF @@ROWCOUNT = 0
			PRINT @statement
	END
	
	-- now lets insert customers for rescoring where they have gone into arrears or been reposessed
	IF @scoretype = 'D'
	BEGIN
		EXEC BlockandRescoreBehaviouralCredit @rescoredate = @curdate
	END
	
	-- here we are storing customers that have been brought back that were originally scored using behavioural scoring
	-- if they now don't qualify because account settled then don't want to continually bring them back
	IF @scoretype = 'D' 
	BEGIN
		INSERT INTO  BehaveRescoreExclude ( custid , datescored)
		SELECT custid,date_rescore FROM customer_rescore
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
	FROM	acct A INNER JOIN 
			proposal P ON A.acctno = P.acctno INNER JOIN
			customer_rescore d ON P.custid = d.custid INNER JOIN
			customer C ON C.custid = d.custid INNER JOIN
			accttype AT ON AT.genaccttype = A.accttype
			/* Could use DATEDIFF(DAY,datelastscored,GETDATE()) > 365 but the following also works for leap years */
	WHERE	GETDATE() > DATEADD(MONTH, CONVERT(integer, @months), C.datelastscored)
	AND		AT.accttype = 'R'
	AND P.propresult = 'A' -- only those which were originally accepted 
	AND		C.creditblocked = 0		/* exclude customers with blocked credit */
	AND		NOT EXISTS (SELECT 	1	/* exclude customers whos RF accounts have expired */
						FROM	custcatcode CC
						WHERE	CC.custid = C.custid 
						AND		CC.code = 'REX' 
						AND		CC.datedeleted is null)
	GROUP BY C.custid, C.RFCreditLimit, C.datelastscored, C.ScoringBand

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
