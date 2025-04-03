-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DuplicateCustomers]') AND type in (N'U'))
BEGIN
	CREATE TABLE DuplicateCustomers
	(
		Custid VARCHAR(20) NOT NULL,
		DuplicateCustid VARCHAR(20) NOT NULL,
		CONSTRAINT [PK_DuplicateCustomers] PRIMARY KEY CLUSTERED
		(
			Custid,
			DuplicateCustid
		)
	)
	
END