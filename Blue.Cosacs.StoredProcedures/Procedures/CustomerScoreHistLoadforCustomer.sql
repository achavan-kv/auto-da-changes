-- updates the proposalBSApply table to mark the scores/Bands as applied. Actual application will be applied at the end of day. 
IF EXISTS (SELECT * FROM sysobjects 
           WHERE NAME ='CustomerScoreHistLoadforCustomer'
           AND xtype = 'P')
BEGIN
	DROP PROCEDURE CustomerScoreHistLoadforCustomer
END
GO 	

CREATE PROCEDURE CustomerScoreHistLoadforCustomer
@custid VARCHAR(20), 
@return INT OUTPUT

AS 
	SET NOCOUNT ON 
	SET @return = 0 

	--SELECT Dateprop AS [Proposal Date],CASE Scorecard WHEN 'B' THEN 'Behavioural'
	--ELSE 'Applicant' 
	--END as ScoreCard,
	SELECT Dateprop AS [Proposal Date],CASE Scorecard 
	WHEN 'D' THEN 'Equifax Behavioural'
	WHEN 'C' THEN 'Equifax Applicant'
	WHEN 'B' THEN 'Behavioural'
	ELSE 'Applicant' 
	END as ScoreCard,
	Points,	CreditLimit AS [Credit Limit],
		Band, DateChange AS [Date Changed],	Empeeno AS [Employee Number], ReasonChanged AS [Reason Changed],Applied
	FROM CustomerScoreHist
	WHERE custid = @custid 
	order by datechange DESC 

	SET @return = @@ERROR
GO 