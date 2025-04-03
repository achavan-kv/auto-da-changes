
SET QUOTED_IDENTIFIER ON -- Very important for this to work.
GO
SET ANSI_NULLS ON -- Very important for this to work.
GO

if exists (select * from dbo.sysobjects 
           where xtype = 'P' 
           and name = 'DN_ScoringRulesGet_InnerSP')
drop procedure DN_ScoringRulesGet_InnerSP
GO

-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : DN_ScoringRulesGet_InnerSP
-- Author			: 
-- Create Date		: 
-- Description		: 
-- Version			: 002					  

-- Change Control
-- --------------
-- Ver	Date			By				Description
-- ---	----			--				-----------
-- 002  05 Feb 2021		Amit (Zensar)	When ApplyNewDispIncomeChanges setting is enabled then 'Monthly Expenses' value need to conside to Zero to overcome on referal message.
-- =======================================================================================


CREATE PROCEDURE DN_ScoringRulesGet_InnerSP
@country char(2),
@scoretype char(1),
@region varchar(3)
AS

	DECLARE @ApplyNewDispIncomeChanges BIT = 0

	SELECT	@ApplyNewDispIncomeChanges = ISNULL([Value], 0) 
	FROM	[dbo].[CountryMaintenance] WITH(NOLOCK)
	WHERE	CodeName ='ApplyNewDispIncomeChanges'

	
	IF(@ApplyNewDispIncomeChanges = 0)
	BEGIN

		SELECT	TOP 1 RulesXML
		FROM	ScoringRules
		WHERE	CountryCode = @country
				AND	Region = @region
				AND ISNULL((CONVERT(XML,rulesxml)).value('(/Rules/@ScoreType)[1]', 'varchar(1)'),'A') = @scoretype -- Select scoring type if not in rules default to application.
		ORDER BY DateImported DESC

	END
	ELSE
	BEGIN
		
		-- In Case of NewDispIncomeChanges setting is enabled then Expense section from Sanction Stage 1 is not available where for expense refer popup is opened.
		-- To overcome on this when this NewDispIncomeChanges setting is enabled then we are changing Rule 'Monthly Expenses' value to Zero
		DECLARE @XMLData XML, @ResultToFind VARCHAR(2)= 'EX', @OperandValueToReplace VARCHAR(10) = '0.00'
	
		SELECT	TOP 1 @XMLData = RulesXML
		FROM	ScoringRules
		WHERE	CountryCode = @country
				AND	Region = @region
				AND ISNULL((CONVERT(XML,rulesxml)).value('(/Rules/@ScoreType)[1]', 'varchar(1)'),'A') = @scoretype -- Select scoring type if not in rules default to application.
		ORDER BY DateImported DESC

		SET @XMLData.modify('replace value of (Rules/Rule[@Result eq sql:variable("@ResultToFind")]/Clause/O2/@Operand)[1] with sql:variable("@OperandValueToReplace")')
		
		SELECT CAST(@XMLData AS VARCHAR(MAX)) AS 'RulesXML'
	END
	
GO

SET QUOTED_IDENTIFIER OFF 
GO
