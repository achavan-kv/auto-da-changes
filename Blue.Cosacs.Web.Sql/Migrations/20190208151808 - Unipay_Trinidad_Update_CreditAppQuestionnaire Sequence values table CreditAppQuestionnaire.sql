
SET XACT_ABORT OFF
GO

/*
-- Script Comment : Updateed Question(description) for Id proof and Sequence values for active questions in table "CreditAppQuestionnaire".
-- Script Name : Unipay_Trinidad_Update_CreditAppQuestionnaire.sql
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
	IF EXISTS (SELECT ScriptName FROM DataFix WHERE ScriptName like 'Unipay_Trinidad_Update_CreditAppQuestionnaire%' AND version = 1)
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
			
			PRINT 'Start Unipay_Trinidad_Update_CreditAppQuestionnaire version 1'
			UPDATE dbo.CreditAppQuestionnaire 
			SET Question = 'Upload front of your ID'
			WHERE QuestionId = 1042
			PRINT 'QuestionId = 1042 Question Updated.'
	
			UPDATE dbo.CreditAppQuestionnaire 
			SET Question = 'Upload back of your ID'
			WHERE QuestionId = 1053
			PRINT 'QuestionId = 1042 Question Updated.'
			
			IF EXISTS(SELECT 1 FROM sys.columns 
								WHERE Name = N'Sequence'
								AND Object_ID = Object_ID(N'dbo.CreditAppQuestionnaire'))
			BEGIN		
			PRINT 'Table update started'
				UPDATE CreditAppQuestionnaire SET [Sequence] =  1 WHERE QuestionId = 1001 -- Enter your DOB
				UPDATE CreditAppQuestionnaire SET [Sequence] =  2 WHERE QuestionId = 1004 -- Enter your Home Address1
				UPDATE CreditAppQuestionnaire SET [Sequence] =  3 WHERE QuestionId = 1009 -- Enter your Marital Status
				UPDATE CreditAppQuestionnaire SET [Sequence] =  4 WHERE QuestionId = 1010 -- Enter your Number of Dependents
				UPDATE CreditAppQuestionnaire SET [Sequence] =  5 WHERE QuestionId = 1011 -- Enter your Nationality
				UPDATE CreditAppQuestionnaire SET [Sequence] =  6 WHERE QuestionId = 1012 -- Enter your Occupation
				UPDATE CreditAppQuestionnaire SET [Sequence] =  7 WHERE QuestionId = 1013 -- Enter your Pay frequency
				UPDATE CreditAppQuestionnaire SET [Sequence] =  8 WHERE QuestionId = 1014 -- Enter your Telephone number
				UPDATE CreditAppQuestionnaire SET [Sequence] =  9 WHERE QuestionId = 1015 -- Enter your Current employment start date
				UPDATE CreditAppQuestionnaire SET [Sequence] = 10 WHERE QuestionId = 1019 -- Enter your Net Income
				UPDATE CreditAppQuestionnaire SET [Sequence] = 11 WHERE QuestionId = 1028 -- Enter your Reference 1 Contact
				UPDATE CreditAppQuestionnaire SET [Sequence] = 12 WHERE QuestionId = 1042 -- Upload your ID proof 1
				UPDATE CreditAppQuestionnaire SET [Sequence] = 13 WHERE QuestionId = 1053 -- Upload your ID proof 2
				UPDATE CreditAppQuestionnaire SET [Sequence] = 14 WHERE QuestionId = 1043 -- Upload your Address proof
				UPDATE CreditAppQuestionnaire SET [Sequence] = 15 WHERE QuestionId = 1044 -- Upload your Income proof
				UPDATE CreditAppQuestionnaire SET [Sequence] = 16 WHERE QuestionId = 1046 -- Enter your Gender
				UPDATE CreditAppQuestionnaire SET [Sequence] = 17 WHERE QuestionId = 1047 -- Enter Current Employer name
				UPDATE CreditAppQuestionnaire SET [Sequence] = 18 WHERE QuestionId = 1048 -- Enter Current Work Address
				UPDATE CreditAppQuestionnaire SET [Sequence] = 19 WHERE QuestionId = 1049 -- Enter Current Resident status
				UPDATE CreditAppQuestionnaire SET [Sequence] = 20 WHERE QuestionId = 1050 -- Enter years at Current Address
				UPDATE CreditAppQuestionnaire SET [Sequence] = 21 WHERE QuestionId = 1052 -- Enter your Loan Expenses
			PRINT 'Table update ended'
			END
		END
		
		--****************************************************************************************************************
		--insert record into datafix table to record change
		INSERT INTO Datafix 
			(ScriptRunDate, ScriptName, 
			Description, 
			Author, Version)
		VALUES 
			(getdate(), 'Unipay_Trinidad_Update_CreditAppQuestionnaire.sql', 
			'Updateed Question(description) for Id proof Sequence values for active questions in table "CreditAppQuestionnaire".',
			'Sagar Kute, Zensar', 1)
		--****************************************************************************************************************

			PRINT 'End Unipay_Trinidad_Update_CreditAppQuestionnaire version 1'
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

SET XACT_ABORT OFF
GO