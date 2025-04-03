-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'HCustid')
BEGIN
	ALTER TABLE [dbo].[InstantCreditApprovalChecks] Alter column HCustid Varchar(20) NOT NULL
END

go

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'PreapprovalDate')
BEGIN
	ALTER TABLE [dbo].[InstantCreditApprovalChecks] Alter column PreapprovalDate DateTime NOT NULL
END

go

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'InstantCreditConditionCheck' AND  Column_Name = 'Custid')
BEGIN
	ALTER TABLE [dbo].[InstantCreditConditionCheck] Alter column Custid Varchar(20) NOT NULL
END

go

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[InstantCreditApprovalChecks]') AND name = N'pk_InstantCreditApprovalChecks')
BEGIN
	ALTER TABLE [dbo].[InstantCreditApprovalChecks] ADD  CONSTRAINT [pk_InstantCreditApprovalChecks] PRIMARY KEY CLUSTERED 
	(
		[HCustid] ASC,
		[PreapprovalDate] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]

END

go

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[InstantCreditConditionCheck]') AND name = N'pk_InstantCreditConditionCheck')
BEGIN
	ALTER TABLE [dbo].[InstantCreditConditionCheck] ADD  CONSTRAINT [pk_InstantCreditConditionCheck] PRIMARY KEY CLUSTERED 
	(
		[Custid] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]

END

go