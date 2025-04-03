-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ICAnalysisReport')

CREATE TABLE ICAnalysisReport
(
	custid varchar(20),
	JCustid VARCHAR(20),
	acctno VARCHAR(12),
	applicationdate DATETIME,
	accttype CHAR(1),
	dateagrmt DATETIME,
	points INT,
	InstantCredit CHAR(1), 
	CreditHistory CHAR(1), 
	AccountStatus CHAR(1), 
	CurrentArrears CHAR(1), 
	HPAccounts CHAR(1), 
	CreditScore CHAR(1), 
	ReferredAccount CHAR(1), 
	LegalAction CHAR(1), 
	ResidenceChanged CHAR(1), 
	EmploymentChanged CHAR(1), 
	JointHolder CHAR(1)

)