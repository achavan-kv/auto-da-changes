
/*
--*********************************************************************** 
-- Script Name : GetCreditAppQuestions.sql 
-- Created For  : Unipay (T) 
-- Created By   : Sagar Kute
-- Created On   : 10/07/2018 
--*********************************************************************** 
-- Change Control 
-- -------------- 
-- Date(DD/MM/YYYY)		Changed By(FName LName)		Description 
-- ------------------------------------------------------------------------------------------------------- 
1. 07/02/2019			Sagar Kute					Updated Stored procedure to order by new column named sequence.
2. 31/07/2019			Sagar Kute					Updated Stored procedure for validation under birth date.
--*********************************************************************************************************

*/

IF OBJECT_ID('dbo.GetCreditAppQuestions') IS NOT NULL
BEGIN
	DROP PROCEDURE dbo.GetCreditAppQuestions
END

GO

CREATE PROCEDURE [dbo].[GetCreditAppQuestions]
	@CustId VARCHAR(20)
	,@Message VARCHAR(MAX) OUTPUT
AS
BEGIN

	SET @Message = ''
	IF(@CustId <> '' AND @CustId IS NOT NULL)
	BEGIN
		IF(NOT EXISTS(SELECT 1 FROM [dbo].[customer] NOLOCK WHERE (LTRIM(RTRIM(CustId)) = @CustId)))
		BEGIN
			SET @Message = 'No questions found for user';
		END
		ELSE
		BEGIN
			SELECT [QuestionId]
					,[Question]
					,[InputType]
					,[InputCategory]
					,[CategorySection]
					,[IsMandatory]
					,ISNULL([Category],'') AS Category
				FROM [dbo].[CreditAppQuestionnaire] NOLOCK
				WHERE IsActive = 1
			ORDER BY [Sequence] ASC

			SELECT 
				LTRIM(RTRIM(code)) AS code, 
				LTRIM(RTRIM(codedescript)) AS codedescript, 
				LTRIM(RTRIM(category)) AS category
			FROM [dbo].[code] NOLOCK
			WHERE category IN (SELECT DISTINCT [Category] FROM [dbo].[CreditAppQuestionnaire] WHERE IsActive = 1 AND [Category] IS NOT NULL)
			--WHERE category IN ('TTL', 'MS1', 'BA2', 'PF1', 'ES1', 'NA2', 'WT1', 'CA1', 'CT1') AND code <> '' 
			-- Title, Marital Status, Account Type, Pay Frequency, Employee Status, Nationality, Occupation, Delivery Area, Address Type
			
			DECLARE @MinHPAge varchar(1500)
			--DECLARE @MinAge int
			SELECT @MinHPAge = Value 
			--Select @MinHPAge = DATEADD(year, -convert(int,value), getdate()), @MinAge = convert(int,value)
			FROM [dbo].[CountryMaintenance] NOLOCK
			WHERE CodeName = 'minhpage'
			
			DECLARE @MaxHPAge varchar(1500) 
			--DECLARE @MaxAge int 
			SELECT @MaxHPAge = Value 
			--SELECT  @MaxHPAge = DATEADD(year, -convert(int,value), getdate()), @MaxAge=convert(int,value)
			FROM [dbo].[CountryMaintenance] NOLOCK
			WHERE CodeName = 'maxhpage'

			SELECT 
				Id,
				QId,
				--CASE WHEN QId = 1001 THEN @MaxHPAge ELSE [Max] END as 'Max',
				--CASE WHEN QId = 1001 THEN @MinHPAge ELSE [Min] END as 'Min',

				-- Swap the Value because of YP required
				CASE WHEN QId = 1001 THEN @MaxHPAge ELSE [Max] END as 'Min',
				CASE WHEN QId = 1001 THEN @MinHPAge ELSE [Min] END as 'Max',
				
				
				--CASE WHEN QId = 1001 THEN @MaxHPAge ELSE DATEADD(year, -convert(int,[Max]), getdate()) END as 'Max',
				--CASE WHEN QId = 1001 THEN @MinHPAge ELSE  DATEADD(year, -convert(int,[Min]), getdate()) END as 'Min',
				Regex,
				replace(MaxErrorMessage,'##age##', Convert(varchar, @MaxHPAge)) As 'MinErrorMessage',
				replace(MinErrorMessage,'##age##', Convert(varchar,@MinHPAge)) As 'MaxErrorMessage',
				RegexErrorMessage
			FROM [dbo].[EMA_Constraints] NOLOCK

			SELECT * 
			FROM [dbo].[EMA_DependsOn] NOLOCK
			
			SELECT * 
			FROM [dbo].[EMA_DependsOnQuestions] NOLOCK
		
			--IF(LTRIM(RTRIM(@Message)) = '')
			SET @Message = 'User and questions found with given details.';
			--ELSE
			--	SET @Message += 'User and questions found with given details.';
		END
	END
	ELSE
	BEGIN
		SET @Message = 'No questions found for user.';		
	END

END
GO

