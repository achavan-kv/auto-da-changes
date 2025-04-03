-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--*************************************************************************************************************	
-- Created By	: Snehalata
-- Created On	: 18-NOV-2020
-- Description  : #7280792 - More than 8 char SKU not able to save through windows client sale Page. Increase datalength for ParentItemNo column.
--*************************************************************************************************************		
IF EXISTS (
			SELECT	1
			FROM	sys.columns
			WHERE	Name = N'ParentItemNo'
					AND Object_ID = Object_ID(N'[dbo].[LineitemAudit]')
			)
BEGIN
		ALTER TABLE [dbo].[LineitemAudit]
		ALTER COLUMN [ParentItemNo] VARCHAR(18)
END
		

