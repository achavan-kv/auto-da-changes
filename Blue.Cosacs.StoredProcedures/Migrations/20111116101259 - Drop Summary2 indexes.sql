-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[summary2_sec]') AND name = N'ix_summary21_sec')
DROP INDEX [ix_summary21_sec] ON [dbo].[summary2_sec] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[summary2_sec]') AND name = N'ix_smrydata2_sec')
DROP INDEX [ix_smrydata2_sec] ON [dbo].[summary2_sec] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[summary2_non]') AND name = N'ix_summary21_non')
DROP INDEX [ix_summary21_non] ON [dbo].[summary2_non] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[summary2_non]') AND name = N'ix_smrydata2_non')
DROP INDEX [ix_smrydata2_non] ON [dbo].[summary2_non] WITH ( ONLINE = OFF )
GO

