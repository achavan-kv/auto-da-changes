-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  not EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_CMStrategyCondition_Strategy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CMStrategyCondition]'))
BEGIN
	ALTER TABLE [dbo].[CMStrategyCondition]  WITH NOCHECK ADD  CONSTRAINT [fk_CMStrategyCondition_Strategy] FOREIGN KEY([Strategy])
	REFERENCES [dbo].[CMStrategy] ([Strategy])
END

GO

IF  not EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_CMStrategyActions_Strategy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CMStrategyActions]'))
BEGIN
	ALTER TABLE [dbo].[CMStrategyActions]  WITH NOCHECK ADD  CONSTRAINT [fk_CMStrategyActions_Strategy] FOREIGN KEY([Strategy])
	REFERENCES [dbo].[CMStrategy] ([Strategy])
END

GO

IF  not EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_CMStrategyAcct_Strategy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CMStrategyAcct]'))
BEGIN
	ALTER TABLE [dbo].[CMStrategyAcct]  WITH NOCHECK ADD  CONSTRAINT [fk_CMStrategyAcct_Strategy] FOREIGN KEY([Strategy])
	REFERENCES [dbo].[CMStrategy] ([Strategy])
END

GO

IF  not EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_CMWorklistsAcct_Strategy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CMWorklistsAcct]'))
BEGIN
	ALTER TABLE [dbo].[CMWorklistsAcct]  WITH NOCHECK ADD  CONSTRAINT [fk_CMWorklistsAcct_Strategy] FOREIGN KEY([Strategy])
	REFERENCES [dbo].[CMStrategy] ([Strategy])
END

GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMWorkList]') AND name = N'pk_WorkList')
ALTER TABLE [dbo].[CMWorkList] DROP CONSTRAINT [pk_WorkList]
GO

alter TABLE CMWorkList alter column Worklist VARCHAR(10) not null

ALTER TABLE [dbo].[CMWorkList] ADD  CONSTRAINT [pk_WorkList] PRIMARY KEY CLUSTERED 
(
	[WorkList] ASC,
	[EmpeeType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


