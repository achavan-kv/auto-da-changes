-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CashLoanDisbursement]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CashLoanDisbursement](
	    [ID] [int] IDENTITY(1,1) NOT NULL,
	    [CustId] [varchar](20) NOT NULL,
	    [AcctNo] [char](12) NOT NULL,
        [AgrmtNo] [int] NOT NULL,
        [LoanAmount] MONEY NOT NULL,
	    [PayMethod] [smallint] NOT NULL,
	    [CardType] [char](1) NULL,
	    [ChequeCardNo] [varchar](16) NULL,
	    [Bank] [varchar](6) NULL,
	    [BankAccountType] [char](1) NULL,
	    [BankBranch] [varchar](20) NULL,
	    [BankAcctNo] [varchar](20) NULL,
	    [Notes] [varchar](200) NULL,
    PRIMARY KEY CLUSTERED 
    (
	    [ID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    CREATE UNIQUE NONCLUSTERED INDEX [IX_CashLoanDisbursement_AcctNo] ON [dbo].[CashLoanDisbursement]
    (
	    [AcctNo] ASC
    )
    INCLUDE (Custid) 
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

