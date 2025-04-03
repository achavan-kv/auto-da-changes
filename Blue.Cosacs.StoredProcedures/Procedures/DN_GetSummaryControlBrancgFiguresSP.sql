SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetSummaryControlBrancgFiguresSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetSummaryControlBrancgFiguresSP]
GO

CREATE PROCEDURE 	dbo.DN_GetSummaryControlBrancgFiguresSP
			@runno int,
			@return int=0 OUTPUT

AS
SET NOCOUNT ON
-- procedure is used from two places letter generation and from normal.net. if from letter generation 
-- then acctno and custid will be passed in blank.

	SET 	@return = 0			--initialise return code

	CREATE TABLE #SummaryBranchFig
	(
		branchno int,
		runopenac int,
		runopenbalance float,
		runcloseac int,
		runclosebalance float,
		runmovement float
	)	

	INSERT INTO #SummaryBranchFig
	  SELECT  DISTINCT  branchno,0,0.0,0,0.0,0.0
	    FROM  interfacevalue 
	   WHERE  runno = @runno
	     AND  interface = 'UPDSMRY'
 	     AND  counttype1 in ('ACCOUNTS B/F','BALANCE B/F',  'ACCOUNTS C/F', 'BALANCE C/F') 
	ORDER BY  branchno

   	UPDATE #SummaryBranchFig
	set runopenac = tmp.startac
	FROM 	 
		(SELECT iv.branchno as branchno, SUM(iv.countvalue) as startac 
		  FROM  interfacevalue iv, #SummaryBranchFig smry
		 WHERE  iv.branchno = smry.branchno 
                   AND  iv.runno = @runno
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'ACCOUNTS B/F'
	      GROUP BY  iv.branchno) as tmp
	      
	WHERE #SummaryBranchFig.branchno = tmp.branchno
	
   	UPDATE #SummaryBranchFig
	set runopenbalance = tmp.runopenbalance
	FROM 	 
		(SELECT iv.branchno as branchno, SUM(iv.value) as runopenbalance 
		  FROM  interfacevalue iv, #SummaryBranchFig smry
		 WHERE  iv.branchno = smry.branchno 
                   AND  iv.runno = @runno
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'BALANCE B/F'
	      GROUP BY  iv.branchno) as tmp
	      
	WHERE #SummaryBranchFig.branchno = tmp.branchno

   	UPDATE #SummaryBranchFig
	set runcloseac = tmp.runcloseac
	FROM 	 
		(SELECT iv.branchno as branchno, SUM(iv.countvalue) as runcloseac 
		  FROM  interfacevalue iv, #SummaryBranchFig smry
		 WHERE  iv.branchno = smry.branchno 
                   AND  iv.runno = @runno
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'ACCOUNTS C/F'
	      GROUP BY  iv.branchno) as tmp
	WHERE #SummaryBranchFig.branchno = tmp.branchno
   	UPDATE #SummaryBranchFig
	set runclosebalance = tmp.runclosebalance
	FROM 	 
		(SELECT iv.branchno as branchno, SUM(iv.value) as runclosebalance 
		  FROM  interfacevalue iv, #SummaryBranchFig smry
		 WHERE  iv.branchno = smry.branchno 
                   AND  iv.runno = @runno
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'BALANCE C/F'
	      GROUP BY  iv.branchno) as tmp
	WHERE #SummaryBranchFig.branchno = tmp.branchno
	
   	UPDATE	#SummaryBranchFig
	SET		runmovement = runopenbalance - runclosebalance

	SELECT	* 
	FROM	#SummaryBranchFig

	IF (@return = 0)
	BEGIN
		SET @return = @return
	END

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO