-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Create table to store paid commission
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bailiffCommnPaid]') AND type in (N'U'))

CREATE TABLE [dbo].[bailiffCommnPaid](
	[empeeno] [int] NOT NULL,
	[transrefno] [int] NOT NULL,
	[acctno] [varchar](12) NULL,
	[datetrans] [datetime] NOT NULL,
	[transvalue] [float] NOT NULL,
	[chequecolln] [char](1) NOT NULL,
	[DatePaid] [datetime] NOT NULL,
 CONSTRAINT [pk_bailiffCommnPaid] PRIMARY KEY CLUSTERED 
(
	[empeeno] ASC,
	[datetrans] ASC,
	[transrefno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO

