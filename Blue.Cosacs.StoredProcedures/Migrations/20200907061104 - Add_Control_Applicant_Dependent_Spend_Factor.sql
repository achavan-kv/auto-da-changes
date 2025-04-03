-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 04 Sept 2020
-- Description		: Add control table entry for Applicant and Dependent Spend Factor.

-- =======================================================================================


GO

DECLARE @taskId INT

IF NOT EXISTS(SELECT 1 FROM control WHERE Control='menuApplicantSpendFactor')
BEGIN
	SELECT @taskId = id FROM [Admin].[Permission] WHERE name ='ViewApplicantSpend'
	
	INSERT INTO CONTROL VALUES (@taskId ,'MainForm' ,'menuApplicantSpendFactor' ,1 ,1 ,'menuSpendFactor')
	INSERT INTO CONTROL VALUES (@taskId ,'MainForm' ,'menuSpendFactor' ,1 ,1 ,'menuSysMaint')
END 

IF NOT EXISTS(SELECT 1 FROM control WHERE Control='menuDepenSpendFactor')
BEGIN
	SELECT @taskId = id FROM [Admin].[Permission] WHERE name ='ViewDependentSpend'
	
	INSERT INTO control VALUES (@taskId ,'MainForm' ,'menuDepenSpendFactor' ,1 ,1 ,'menuSpendFactor')
	INSERT INTO control VALUES (@taskId ,'MainForm' ,'menuSpendFactor' ,1 ,1 ,'menuSysMaint')
END

GO