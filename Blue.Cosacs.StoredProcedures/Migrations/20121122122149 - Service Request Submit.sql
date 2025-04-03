-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #10912

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SR_Summary]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[SR_Summary](
	[Acctno] [char](12) NOT NULL,
	[ServiceRequestNo] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[DateLogged] [datetime] NULL,
	[DateClosed] [datetime] NOT NULL
 CONSTRAINT [pk_SR_Summary] PRIMARY KEY CLUSTERED 
(
	[Acctno] ASC,
	[ServiceRequestNo] ASC
))

END
