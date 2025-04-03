-- updates the proposalBSApply table to mark the scores/Bands as applied. Actual application will be applied at the end of day. 
IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='ProposalBSApply')
	DROP PROCEDURE ProposalBSApply
GO 	
CREATE PROCEDURE dbo.ProposalBSApply
-- **********************************************************************
-- Title: ProposalBSApply.sql
-- Developer: ??
-- Date: ??
-- Purpose: 

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 10/05/11  IP  #3607 - ScoreCardType was not being updated to 'B' to reflect behavioural
-- **********************************************************************
@custid      VARCHAR(20), --Rejected, Lower, Higher,Same ,Band
@runno INT ,
@return INT OUTPUT 
AS
	SET NOCOUNT ON 
	SET @return = 0 
	SELECT @runno= MAX(runno) 
	FROM proposalBS WHERE custid = @custid 
	UPDATE proposalbs SET applied = 'A' 
	WHERE custid = @custid AND runno = @runno 

	UPDATE customer SET ScoringBand = p.ScoringBand,
	ScoreCardType = 'B'	--IP - 10/05/11 - #3607
	FROM proposalbs p 
	WHERE p.custid = customer.custid AND p.runno = @runno 
	
	UPDATE CustomerScoreHist 
	SET applied= 1
	FROM proposalbs  p
	WHERE  p.custid = CustomerScoreHist.Custid AND p.runno= @runno 
	AND CustomerScoreHist.applied = 0

GO 