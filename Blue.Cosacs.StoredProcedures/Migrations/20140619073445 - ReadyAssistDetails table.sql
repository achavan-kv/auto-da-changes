-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #18603 - CR15594


IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ReadyAssistDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE ReadyAssistDetails(
    [AcctNo] [varchar](12) NOT NULL,
	[AgrmtNo] [int] NOT NULL,
	[RAContractDate] [date] NULL,
	[RATermLength] [int] NULL,
 CONSTRAINT [pk_ReadyAssistDetails] PRIMARY KEY CLUSTERED 
(
	[AcctNo] ASC,
	[AgrmtNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO