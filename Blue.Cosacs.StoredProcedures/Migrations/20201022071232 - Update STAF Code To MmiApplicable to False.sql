-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- =======================================================================================
-- Author			: Amit Vernekar
-- Create Date		: 22 OCt 2020
-- Description		: Set IsMmiApplicable False to code STAF
-- =======================================================================================

GO

UPDATE CodeConfiguration
SET	   IsMmiApplicable = 0
WHERE  Code = 'STAF'

GO