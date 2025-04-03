/*
-- Script Comment : Added "Sequence" column in table "CreditAppQuestionnaire".
-- Script Name : Unipay_Trinidad_Alter.sql
-- Script Version : 1
-- Created For	: Trinidad
-- Created By	: Sagar Kute
-- Created On	: 02/07/2019
--
--
-- Modified On	Modified By	Comment
*/

SET XACT_ABORT ON
Go
SET IMPLICIT_TRANSACTIONS OFF
Go

BEGIN TRAN
	--check if script has already been run
	IF EXISTS (SELECT ScriptName FROM DataFix WHERE ScriptName like 'Unipay_Trinidad_Alter%' AND version = 1)
	BEGIN
		PRINT 'This script has already been run and can not be run twice. Please contact CoSACS Support Centre.'
	END 
	ELSE
	BEGIN
		--*************************************************************************************************************	
		IF (EXISTS (SELECT 1 
						FROM sysobjects 
						WHERE NAME = 'CreditAppQuestionnaire'))
		BEGIN
			
			PRINT 'Start version 1'
			
			IF NOT EXISTS(SELECT 1 FROM sys.columns 
								WHERE Name = N'Sequence'
								AND Object_ID = Object_ID(N'dbo.CreditAppQuestionnaire'))
			BEGIN
			PRINT 'Table Alter started'
				ALTER TABLE CreditAppQuestionnaire 
				ADD [Sequence] INT NOT NULL DEFAULT 999
			PRINT 'Table Alter completed'
			END

		--****************************************************************************************************************
		--insert record into datafix table to record change
		INSERT INTO Datafix 
			(ScriptRunDate, ScriptName, 
			Description, 
			Author, Version)
		VALUES 
			(getdate(), 'Unipay_Trinidad_Alter.sql', 
			'Added "Sequence" column and updated seques values for active questions in table "CreditAppQuestionnaire".',
			'Sagar Kute, Zensar', 1)
		--****************************************************************************************************************

			PRINT 'End version 1'
	END
END
IF @@ERROR > 0
BEGIN
	ROLLBACK
END
ELSE
BEGIN
	COMMIT
END
GO

-- ===================================================================================================================================
