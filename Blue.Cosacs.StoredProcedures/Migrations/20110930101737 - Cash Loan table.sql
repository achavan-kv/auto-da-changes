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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CashLoan]') AND type in (N'U'))
DROP TABLE [dbo].[CashLoan]
GO

CREATE TABLE [dbo].[CashLoan]
(
	Custid		VARCHAR(20),
	AcctNo		CHAR(12),
	LoanAmount	MONEY,
	Term		INT,
	LoanStatus	CHAR(1)

CONSTRAINT [pk_CashLoan] PRIMARY KEY CLUSTERED 
(
	[Custid] ASC,
	[AcctNo] ASC
)
) ON [PRIMARY]
