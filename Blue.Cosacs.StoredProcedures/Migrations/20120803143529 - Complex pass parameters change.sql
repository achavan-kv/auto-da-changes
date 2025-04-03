-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DELETE CountryMaintenance WHERE CodeName IN('MinRequiredPasswordLength', 'PasswordExpireDays')
UPDATE CountryMaintenance SET ParameterCategory = '17' WHERE CodeName = 'MinRequiredNonalphanumericChar'
GO

ALTER VIEW [Admin].[GetPasswordComplexityParameters]
AS
	SELECT TOP 100 PERCENT
		Value, 
		CodeName
	FROM
	(
		SELECT
			Value, 
			CodeName,
			1 AS OrderField
		FROM         
			CountryMaintenance AS t
		WHERE     
			ParameterCategory = '17' 
			AND CodeName ='MinRequiredNonalphanumericChar'
		UNION ALL
		SELECT
			Value, 
			CodeName,
			2 AS OrderField
		FROM         
			CountryMaintenance AS t
		WHERE     
			ParameterCategory = '17' 
			AND CodeName = 'PasswordMinLength'
		UNION ALL
		SELECT
			Value, 
			CodeName,
			3 AS OrderField
		FROM         
			CountryMaintenance AS t
		WHERE     
			ParameterCategory = '17' 
			AND CodeName = 'passwordchangedays'
	) AS Params
	ORDER BY
		OrderField
GO