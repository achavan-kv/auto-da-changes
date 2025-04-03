-- updates the proposalBSApply table to mark the scores/Bands as applied. Actual application will be applied at the end of day. 
IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='ProposalBSApplyRescore')
	DROP PROCEDURE ProposalBSApplyRescore
GO 	
CREATE PROCEDURE dbo.ProposalBSApplyRescore
-- **********************************************************************
-- Title: ProposalBSApplyRescore.sql
-- Developer: ??
-- Date: ??
-- Purpose: 

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 10/05/11  IP  #3607 - ScoreCardType was not being updated to 'B' to reflect behavioural
-- **********************************************************************
	@return INT OUTPUT 
AS

	SET NOCOUNT ON 
	SET @return = 0 
--	SELECT TOP 3 * FROM  ProposalBS

	 SELECT b.acctno, p.scoringBand AS oldBand,B.scoringBand AS NewBand,
	 c.rfcreditlimit AS oldlimit, b.newlimit  ,CONVERT(VARCHAR(4),'') AS lettercode 
	 INTO #totals
	 FROM proposalBS b 
	 JOIN proposal p ON b.acctno= p.acctno AND b.custid = p.custid 
	 AND p.dateprop = b.dateprop 
	 JOIN customer c ON c.custid = b.custid 
	 WHERE applied ='A' --AND runno= @runno 
	 
	UPDATE #totals SET lettercode = 'BHHL' 
	WHERE  newlimit > oldlimit 
	
	-- Old Bands 
	UPDATE #totals SET lettercode = 'BHBI' 
	WHERE  newlimit = oldlimit AND ISNUMERIC(oldband) =1
	AND newband <oldband
	
	UPDATE #totals SET lettercode = 'BHHB' 
	WHERE  newlimit > oldlimit AND ISNUMERIC(oldband) =1
	AND newband <oldband
	
	--SELECT * FROM #totals

	DECLARE @runno int

	SELECT @runno = runno 
	FROM interfacecontrol
	WHERE interface = 'BHSRescore'

	/*Now going to install*/
	INSERT INTO letter (
		runno,		acctno,		dateacctlttr,
		datedue,		lettercode,
		addtovalue,		ExcelGen
	) 
	
	SELECT @runno,acctno,GETDATE(),
	DATEADD(DAY,14,GETDATE()),lettercode,
	0, 0 
	FROM #totals
	WHERE  ISNULL(lettercode,'') !=''
	
			
	UPDATE proposal 
	SET points = b.points,
	proposal.reason = b.reason,
	proposal.reason2 = b.reason2,
	proposal.reason3 = b.reason3,
	proposal.reason4 = b.reason4,
	proposal.reason5 = b.reason5,
	proposal.reason6 = b.reason6
	--propresult = b.propresult,  don't want it showing referred
	--ScoringBand = b.ScoringBand -- scoring band should remain on original account...
	FROM proposalBS b WHERE 
	--b.runno= @runno AND
	 proposal.custid = b.custid 
	AND b.applied = 'A'
	
	UPDATE customer 
	SET ScoringBand = b.ScoringBand, -- but do apply new band
	RFCreditLimit = b.newlimit, -- and new credit limit
	ScoreCardType = 'B'		--IP - 10/05/11 - #3607
	FROM proposalBS b WHERE 
	--b.runno= @runno 	AND 
	customer.custid = b.custid 
	AND b.applied = 'A'
	and b.propresult NOT IN ('X')

	UPDATE proposalBS SET applied = 'Y' 
	WHERE --runno= @runno AND 
	applied ='A'
	
	 
GO   

