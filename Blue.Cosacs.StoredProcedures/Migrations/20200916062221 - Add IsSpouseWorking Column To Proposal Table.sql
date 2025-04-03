-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- *************************************************************************************************
-- Developer:	Shubham Gaikwad
-- Date:		15 Sep 2020
-- Purpose:		Add IsSpouseWorking column in Proposal table.
-- *************************************************************************************************


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Proposal' AND Column_Name = 'IsSpouseWorking')
BEGIN
	ALTER TABLE Proposal ADD IsSpouseWorking BIT NULL

	ALTER TABLE Proposal ADD CONSTRAINT DF_Proposal_IsSpouseWorking DEFAULT 0 FOR IsSpouseWorking

END
GO


UPDATE	Proposal
SET		IsSpouseWorking = 0
WHERE	IsSpouseWorking IS NULL




