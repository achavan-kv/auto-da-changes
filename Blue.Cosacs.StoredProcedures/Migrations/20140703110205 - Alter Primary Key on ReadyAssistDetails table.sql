-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ReadyAssistDetails]') AND name = N'pk_ReadyAssistDetails')
BEGIN

	ALTER TABLE [dbo].[ReadyAssistDetails] DROP CONSTRAINT [pk_ReadyAssistDetails]

	ALTER TABLE [dbo].[ReadyAssistDetails] ADD  CONSTRAINT [pk_ReadyAssistDetails] PRIMARY KEY CLUSTERED 
	(
		[AcctNo] ASC,
		[AgrmtNo] ASC,
		[ContractNo] ASC
	) ON [PRIMARY]

END


