-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sysobjects 
              WHERE name = 'IgnoreCRECRF'
			  AND xtype = 'U')
BEGIN
	CREATE TABLE IgnoreCRECRF
	(
		AcctNo VARCHAR(12) NOT NULL,
		ContractNo VARCHAR(10) NOT NULL,
		CONSTRAINT [PK_IgnoreCRECRF] PRIMARY KEY CLUSTERED
		(
			AcctNo,
			ContractNo
		)
	)
	
END