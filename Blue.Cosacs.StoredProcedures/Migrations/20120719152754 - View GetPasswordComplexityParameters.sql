-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE VIEW [Admin].GetPasswordComplexityParameters
AS
	SELECT
		Value, 
		CodeName
	FROM         
		CountryMaintenance AS t
	WHERE     
		ParameterCategory = '01' 
		AND CodeName IN ('MinRequiredNonalphanumericChar', 'MinRequiredPasswordLength', 'PasswordExpireDays')
GO



