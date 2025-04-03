-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FinancialWeeks]') AND type in (N'U'))
DROP TABLE [dbo].[FinancialWeeks]
GO

CREATE TABLE [dbo].[FinancialWeeks](
	[Year] [smallint] NOT NULL,
	[Week] [tinyint] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Quarter] [tinyint] NULL,
	[DaysCount] [tinyint] NULL,
 CONSTRAINT [PK_FinancialWeeks] PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[Week] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
