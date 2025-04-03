-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID('InstantCreditApprovalsCheckGen_Val') AND TYPE IN (N'U'))
BEGIN
	CREATE TABLE InstantCreditApprovalsCheckGen_Val
	(piCustomerID		VARCHAR(20) not null,
	piAccountNo		VARCHAR(12) not null,
	poInstantCredit	CHAR(1) null,
	poLoanQualified	CHAR(1) null,
	ApprovlDate DATETIME  not null)
	

	ALTER TABLE InstantCreditApprovalsCheckGen_Val ADD PRIMARY KEY ( piCustomerID, ApprovlDate, piAccountNo );
	ALTER TABLE InstantCreditApprovalsCheckGen_Val ADD CONSTRAINT df_ApprovlDate DEFAULT getdate() for ApprovlDate;
END
