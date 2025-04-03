IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'AutoPOJobHistory'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[AutoPOJobHistory](
	[JobName] [sysname] NOT NULL,
	[IsRunning] [int] NOT NULL,
	[RequestSource] [sysname] NULL,
	[LastRunTime] [datetime] NULL,
	[NextRunTime] [datetime] NULL,
	[LastJobStep] [sysname] NULL,
	[RetryAttempt] [int] NULL,
	[JobLastOutcome] [varchar](9) NULL,
	[LastError] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO