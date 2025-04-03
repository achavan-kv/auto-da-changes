
GO

IF NOT EXISTS (
				 SELECT	1
				 FROM	sys.Tables WITH (NOLOCK)
				 WHERE	NAME = 'CMStrategyCondition_Bkp_ARB_ARR'
						AND TYPE = 'U'
			 )
BEGIN
		
		SELECT	* 
		INTO	CMStrategyCondition_Bkp_ARB_ARR  
		FROM	[dbo].[CMStrategyCondition] 
		WHERE	Strategy in ('ARB','ARR')

		DELETE FROM [dbo].[CMStrategyCondition] WHERE Strategy IN ('ARB','ARR')
		DELETE FROM [dbo].[CMStrategyCondition] WHERE Strategy NOT IN (SELECT Strategy FROM CMStrategy)
END


IF NOT EXISTS	(
				SELECT	1
				FROM	sys.Tables WITH (NOLOCK)
				WHERE	NAME = 'CMStrategyActions_Bkp_ARB_ARR'
						AND TYPE = 'U'
				)
BEGIN
		
		SELECT	* 
		INTO	CMStrategyActions_Bkp_ARB_ARR  
		FROM	[dbo].[CMStrategyActions] 
		WHERE	Strategy in ('ARB','ARR')

		DELETE FROM [dbo].[CMStrategyActions] WHERE Strategy IN ('ARB','ARR')
		DELETE FROM [dbo].[CMStrategyActions] WHERE Strategy NOT IN (SELECT Strategy FROM CMStrategy)
END

GO


