-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT TOP 1 IsMmiApplicable FROM [dbo].[CodeConfiguration] WITH(NOLOCK))
BEGIN

	INSERT INTO CodeConfiguration (Category, Code, IsMmiApplicable)
	SELECT	C.Category, C.Code, 1
	FROM	[dbo].Code AS C	
	WHERE	C.statusflag = 'L' 
			AND	C.Category IN ('CC1','CC2')

END
