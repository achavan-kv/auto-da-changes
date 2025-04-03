-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11838

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServiceChargeAcct]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[ServiceChargeAcct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServiceRequestNo] [int] NOT NULL,
	[AcctNo] [char](12) NOT NULL,
	[ChargeType] [char](1) NOT NULL,
	CONSTRAINT [pk_ServiceChargeAcct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [Unq_ServiceChargeAcct] UNIQUE NONCLUSTERED 
(
	[ServiceRequestNo] ASC,
	[AcctNo]  ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
