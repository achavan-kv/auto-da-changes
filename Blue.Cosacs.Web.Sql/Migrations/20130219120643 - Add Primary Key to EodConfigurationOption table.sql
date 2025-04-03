-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12341 - CR11571

IF  NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[EodConfigurationOption]') AND name = N'pk_EodConfigurationOption')
BEGIN
	ALTER TABLE [dbo].[EodConfigurationOption] ADD  CONSTRAINT [pk_EodConfigurationOption] PRIMARY KEY CLUSTERED 
	(
		[ConfigurationName] ASC,
		[OptionCode] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]

END