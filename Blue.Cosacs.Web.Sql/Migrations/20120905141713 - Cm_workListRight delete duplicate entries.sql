-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12771

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'CMWorkListRights' 
           AND COLUMN_NAME = 'Empeetype')
BEGIN

	SELECT empeeno, worklist, 0 AS empeenochange
	INTO #CMWorkListRights_temp
	FROM [CMWorkListRights]
	GROUP BY empeeno, worklist
	HAVING COUNT(*) > 1

	UPDATE #CMWorkListRights_temp
	SET empeenochange = c.empeenochange
	FROM CMWorkListRights C INNER JOIN #CMWorkListRights_temp T ON c.empeeno = t.empeeno
							AND c.worklist = t.worklist

	DELETE CMWorkListRights
	FROM CMWorkListRights c
	INNER JOIN #CMWorkListRights_temp T ON T.empeeno = c.Empeeno AND T.Worklist = c.WorkList

	INSERT INTO dbo.CMWorkListRights
	SELECT c.empeeno, c.worklist, '', c.empeenochange
	FROM #CMWorkListRights_temp c

END
