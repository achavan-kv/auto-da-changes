-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 24 Sept 2020
-- Description		: Insert values in MandatoryFields for chxSpouseWorking field
-- 
-- =======================================================================================

IF NOT EXISTS (SELECT 1 FROM MandatoryFields WHERE screen='SanctionStage1' AND control='chxSpouseWorking')
BEGIN 

	INSERT INTO MandatoryFields (country, screen, control, description, enabled, visible, mandatory)
	VALUES(	(Select countrycode from country),
			'SanctionStage1',
			'chxSpouseWorking',
			'Is Applicants Spouse working',
			1,	1,	0)

END