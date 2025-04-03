SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetSummaryUpdateControlDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetSummaryUpdateControlDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_GetSummaryUpdateControlDetailsSP
			@firstrunno int,
			@lastrunno int,
			@return int=0 OUTPUT

AS
SET NOCOUNT ON
-- procedure is used from two places letter generation and from normal.net. if from letter generation 
-- then acctno and custid will be passed in blank.

	declare @start int 
	declare @end int 

	SET 	@return = 0			--initialise return code
	SET 	@start = @firstrunno
	SET     @end =  @lastrunno

	CREATE TABLE #SummaryControl
	(
		batchno int,
		runstart datetime,
		runend datetime,
		runopenac int,
		runopenbalance float,
		runcloseac int,
		runclosebalance float,
		runmovement float
	)	
	IF @firstrunno = 0
	BEGIN
		SELECT 	@start = MAX(runno)-10
		  FROM  interfacecontrol ic
		 WHERE  ic.interface = 'UPDSMRY' 
		SET @end = @start +10
	END
	INSERT INTO #SummaryControl
		SELECT  ic.runno, ic.datestart, ic.datefinish, 0, 0.0,0, 0.0,0.0
	    	  FROM  interfacecontrol ic
	 	 WHERE  ic.runno  >= @start
	           AND  ic.runno <= @end
	           AND  ic.interface = 'UPDSMRY'
	           AND	 result in ('P','W')

   	UPDATE #SummaryControl
	set runopenac = tmp.startac
	FROM 	 
		(SELECT  iv.runno as runno, SUM (iv.countvalue) as startac 
		  FROM  interfacevalue iv, #SummaryControl smry
		 WHERE  iv.runno = smry.batchno 
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'ACCOUNTS B/F' 
	      GROUP BY  iv.runno) as tmp
	WHERE #SummaryControl.batchno = tmp.runno

   	UPDATE #SummaryControl
	set runopenbalance = tmp.startbalance
	FROM 	 
		(SELECT  iv.runno as runno, SUM (iv.value) as startbalance 
	  FROM  interfacevalue iv, #SummaryControl smry
		 WHERE  iv.runno = smry.batchno 
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'BALANCE B/F' 
	      GROUP BY  iv.runno) as tmp
	WHERE #SummaryControl.batchno = tmp.runno

   	UPDATE #SummaryControl
	set runcloseac = tmp.endac
	FROM 	 
		(SELECT  iv.runno as runno, SUM (iv.countvalue) as endac 
		  FROM  interfacevalue iv, #SummaryControl smry
		 WHERE  iv.runno = smry.batchno 
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'ACCOUNTS C/F' 
	      GROUP BY  iv.runno) as tmp
	WHERE #SummaryControl.batchno = tmp.runno

   	UPDATE #SummaryControl
	set runclosebalance = tmp.endbalance
	FROM 	 
		(SELECT  iv.runno as runno, SUM (iv.value) as endbalance 
		  FROM  interfacevalue iv, #SummaryControl smry
		 WHERE  iv.runno = smry.batchno 
		   AND  iv.interface = 'UPDSMRY'
		   AND  iv.counttype1 =  'BALANCE C/F' 
	      GROUP BY  iv.runno) as tmp
	WHERE #SummaryControl.batchno = tmp.runno
	
   	UPDATE	#SummaryControl
	SET		runmovement = runopenbalance - runclosebalance
	
	SELECT  * from    #SummaryControl ORDER BY batchno DESC

	IF (@return = 0)
	BEGIN
		SET @return = @return
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO