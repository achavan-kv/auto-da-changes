
IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'ScoreTypeToUse'
		   AND xtype = 'p')
BEGIN
	DROP PROCEDURE ScoreTypeToUse
END
GO
CREATE PROCEDURE [dbo].[ScoreTypeToUse]

@custid VARCHAR(20),
 @acctno CHAR(12), 
 @scorecard CHAR(1) OUTPUT , 
 @return INT OUTPUT
AS 
-- determining whether the customer has enough behavioural history to use the behavioural scorecard
SET @return = 0 
DECLARE @scoretype CHAR(1)
--'This parameter for scoring process. A - Applicant ScoreCards, B - Behavioural Scoring (branch activated), P - Parallel (Behavioural and Applicant), S - Behavioural Scoring 
SELECT @scoretype=value FROM CountryMaintenance WHERE codename = 'BehaviouralScorecard'

BEGIN
---------------------------------- VARIABLE SECTION 
		DECLARE @EvaluationMonths INT , @minActivityMonths INT , @evaluationDatefrom DATETIME 
		DECLARE @acctypetouse CHAR(1), @accttype CHAR(1)
		DECLARE @accts TABLE (acctno CHAR(12), datefrom DATETIME,dateto DATETIME, qualifyperiod INT )
		DECLARE @totalperiod SMALLINT
-----------------------------
	--OLD SCORE CARD CONDITION
	IF (@scoretype = 'A' or @scoretype = 'B'  or @scoretype = 'P' or @scoretype = 'S')
	BEGIN
		IF @scoretype = 'A'
		BEGIN
			SET @scorecard = 'A' -- default applicant
			RETURN 
		END
		IF @scoretype = 'B'	
		BEGIN
			IF NOT EXISTS (SELECT * FROM branch WHERE branchno = CONVERT(INT,LEFT(@acctno,3)) AND BehaviouralScoring = 1)	
			BEGIN
				SET @scorecard = 'A' -- default applicant as this branch not doing behavioural
				RETURN 
			END
		END
		--DECLARE @EvaluationMonths INT , @minActivityMonths INT , @evaluationDatefrom DATETIME 
		--DECLARE @acctypetouse CHAR(1), @accttype CHAR(1)
		--- accttype to use 0 = Both 1 = RF only 2 = HP only
		SELECT @acctypetouse= value FROM CountryMaintenance WHERE name = 'Behavioural Accounts'
		SELECT @accttype =accttype FROM acct WHERE acctno= @acctno 

		IF (@accttype = 'R' AND @acctypetouse= '2') 
		OR (@accttype !='R' AND @acctypetouse='1' )
		BEGIN
			SET @scorecard = 'A'
			RETURN 
		END

		SELECT @minactivitymonths= CONVERT(INT,value) FROM CountryMaintenance WHERE codename = 'BehaviouralMonthsHistory'
		SELECT @EvaluationMonths= value FROM CountryMaintenance WHERE codename = 'BehaviouralMonthsPeriod'

		SET	 @evaluationDatefrom  = DATEADD(MONTH,-@EvaluationMonths,GETDATE()) 

		--DECLARE @accts TABLE (acctno CHAR(12), datefrom DATETIME,dateto DATETIME, qualifyperiod INT ) 

		INSERT INTO @accts
		(acctno,datefrom,dateto) 
		SELECT ca.acctno,i.DATEFIRST,NULL 
		FROM custacct ca	
		JOIN instalplan i ON ca.acctno = i.acctno
		JOIN agreement g ON i.acctno= g.acctno 
		WHERE ca.hldorjnt= 'H' AND ca.custid = @custid 
		AND ISNULL(g.datedel,'1-jan-1900') > '1-jan-1900'
		-- so if account settled get datesettled
		UPDATE a SET dateto =ISNULL((SELECT Min(datestatchge) -- using minimum cos if settled later then account not really open in period
		FROM status s WHERE s.acctno= a.acctno AND s.statuscode = 'S'
		), GETDATE()) 
		FROM @accts a , acct b 
		WHERE a.acctno= b.acctno AND b.outstbal = 0 
		-- remove accounts settled priod to evaluation period 
		DELETE FROM @accts WHERE dateto < @evaluationDatefrom 
		-- looking at last x months of data so earlier dates not included...
		UPDATE @accts SET datefrom = @evaluationDatefrom WHERE datefrom < @evaluationDatefrom

		UPDATE A 
		SET dateto = getdate()
		FROM @accts a ,acct b 
		WHERE a.acctno = b.acctno AND b.outstbal > 0
		-- now we need to make sure if overlapping periods then not counted twice....
		/* So for example if customer has two accounts 
		1. from 3-jan-2009 -- 5th June 2009
		2. from 1-dec-2009 -  3rd Feb 2009
		3. And say today was  3rd dec 2010 - -then 2nd account date to should be first account datefrom....
		*/

		UPDATE a 
		SET dateto =b.datefrom 
		FROM @accts a, @accts b 
		WHERE a.acctno !=b.acctno 
		and a.dateto > b.datefrom AND a.dateto < b.dateto 

		UPDATE a 
		SET datefrom =b.dateto
		FROM @accts a, @accts b 
		WHERE a.acctno !=b.acctno 
		and a.datefrom < b.dateto AND a.datefrom > b.datefrom 

		UPDATE @accts SET qualifyperiod = DATEDIFF(MONTH,datefrom,dateto)
		--DECLARE @totalperiod SMALLINT
		SELECT @totalperiod =SUM(qualifyperiod) FROM @accts
		SELECT @totalperiod AS TOTAL, @EvaluationMonths AS MONTHS 
		IF @totalperiod >= @minActivityMonths 
			SET @scorecard ='B'
		ELSE -- default applicant
			SET @scorecard ='A'	

		IF @scoretype = 'P' AND @scorecard='B' 
			SET @scorecard ='P' 
	END

	ELSE  --- EQUIFAX SCORE CARD
	BEGIN
	-------------Equifax condition
	
	IF @scoretype = 'C'
		BEGIN
			SET @scorecard = 'C' -- default applicant
			RETURN 
		END
		IF @scoretype = 'D'	
		BEGIN
			IF NOT EXISTS (SELECT * FROM branch WHERE branchno = CONVERT(INT,LEFT(@acctno,3)) AND BehaviouralScoring = 1)	
			BEGIN
				SET @scorecard = 'C' -- default applicant as this branch not doing behavioural
				RETURN 
			END
		END
		--DECLARE @EvaluationMonths INT , @minActivityMonths INT , @evaluationDatefrom DATETIME 
		--DECLARE @acctypetouse CHAR(1), @accttype CHAR(1)
		--- accttype to use 0 = Both 1 = RF only 2 = HP only
		SELECT @acctypetouse= value FROM CountryMaintenance WHERE name = 'Behavioural Accounts'
		SELECT @accttype =accttype FROM acct WHERE acctno= @acctno 

		IF (@accttype = 'R' AND @acctypetouse= '2') 
		OR (@accttype !='R' AND @acctypetouse='1' )
		BEGIN
			SET @scorecard = 'C'
			RETURN 
		END

		SELECT @minactivitymonths= CONVERT(INT,value) FROM CountryMaintenance WHERE codename = 'BehaviouralMonthsHistory'
		SELECT @EvaluationMonths= value FROM CountryMaintenance WHERE codename = 'BehaviouralMonthsPeriod'

		SET	 @evaluationDatefrom  = DATEADD(MONTH,-@EvaluationMonths,GETDATE()) 

		--DECLARE @accts TABLE (acctno CHAR(12), datefrom DATETIME,dateto DATETIME, qualifyperiod INT ) 

		INSERT INTO @accts
		(acctno,datefrom,dateto) 
		SELECT ca.acctno,i.DATEFIRST,NULL 
		FROM custacct ca	
		JOIN instalplan i ON ca.acctno = i.acctno
		JOIN agreement g ON i.acctno= g.acctno 
		WHERE ca.hldorjnt= 'H' AND ca.custid = @custid 
		AND ISNULL(g.datedel,'1-jan-1900') > '1-jan-1900'
		-- so if account settled get datesettled
		UPDATE a SET dateto =ISNULL((SELECT Min(datestatchge) -- using minimum cos if settled later then account not really open in period
		FROM status s WHERE s.acctno= a.acctno AND s.statuscode = 'S'
		), GETDATE()) 
		FROM @accts a , acct b 
		WHERE a.acctno= b.acctno AND b.outstbal = 0 
		-- remove accounts settled priod to evaluation period 
		DELETE FROM @accts WHERE dateto < @evaluationDatefrom 
		-- looking at last x months of data so earlier dates not included...
		UPDATE @accts SET datefrom = @evaluationDatefrom WHERE datefrom < @evaluationDatefrom

		
		UPDATE A 
		SET dateto = getdate()
		FROM @accts a ,acct b 
		WHERE a.acctno = b.acctno AND b.outstbal > 0
		-- now we need to make sure if overlapping periods then not counted twice....
		/* So for example if customer has two accounts 
		1. from 3-jan-2009 -- 5th June 2009
		2. from 1-dec-2009 -  3rd Feb 2009
		3. And say today was  3rd dec 2010 - -then 2nd account date to should be first account datefrom....
		*/

		UPDATE a 
		SET dateto =b.datefrom 
		FROM @accts a, @accts b 
		WHERE a.acctno !=b.acctno 
		and a.dateto > b.datefrom AND a.dateto < b.dateto 

		UPDATE a 
		SET datefrom =b.dateto
		FROM @accts a, @accts b 
		WHERE a.acctno !=b.acctno 
		and a.datefrom < b.dateto AND a.datefrom > b.datefrom 

		UPDATE @accts SET qualifyperiod = DATEDIFF(MONTH,datefrom,dateto)
		--DECLARE @totalperiod SMALLINT
		SELECT @totalperiod =SUM(qualifyperiod) FROM @accts
		SELECT @totalperiod AS TOTAL, @EvaluationMonths AS MONTHS 

		
	--update CountryMaintenance set value=17  WHERE codename = 'BehaviouralMonthsPeriod'
	--update CountryMaintenance set value=0  WHERE codename = 'BehaviouralMonthsHistory'

		IF @totalperiod >= @minActivityMonths 
			SET @scorecard ='D'
		ELSE -- default applicant
			SET @scorecard ='C'	

		IF @scoretype = 'E' AND @scorecard='D'   --E FOR EQUIFAX PARALLEL
			SET @scorecard ='E'

	
	-------------- EQUIFAX END
	END

END