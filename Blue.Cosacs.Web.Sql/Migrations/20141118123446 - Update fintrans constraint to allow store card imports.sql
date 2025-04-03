-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [dbo].[fintrans] DROP CONSTRAINT [fintrans_source_check]
GO

ALTER TABLE [dbo].[fintrans]  WITH NOCHECK ADD  CONSTRAINT [fintrans_source_check] CHECK  (([source]='COASTER' OR [source]='COSACS' OR [Source]='SINGER' OR [Source]='SCImport'))
GO

ALTER TABLE [dbo].[fintrans] CHECK CONSTRAINT [fintrans_source_check]
GO


