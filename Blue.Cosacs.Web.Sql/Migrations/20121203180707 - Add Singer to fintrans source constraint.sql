-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[fintrans_source_check]') AND parent_object_id = OBJECT_ID(N'[dbo].[fintrans]'))
ALTER TABLE [dbo].[fintrans] DROP CONSTRAINT [fintrans_source_check]
GO

ALTER TABLE [dbo].[fintrans]  WITH NOCHECK ADD  CONSTRAINT [fintrans_source_check] CHECK  (([source] = 'COASTER' or [source] = 'COSACS' OR [Source] = 'SINGER'))
GO

ALTER TABLE [dbo].[fintrans] CHECK CONSTRAINT [fintrans_source_check]
GO


