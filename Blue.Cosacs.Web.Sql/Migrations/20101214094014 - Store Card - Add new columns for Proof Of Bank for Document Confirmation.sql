-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Add new columns to the Proposal table to record Proof Of Bank for Store Card from Document Confirmation
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='Proposal' AND column_name = 'ProofOfBank')
    alter table Proposal add ProofOfBank varchar(4) null
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='Proposal' AND column_name = 'ProofOfBankTxt')
	alter table Proposal add ProofOfBankTxt varchar(350) null
go
