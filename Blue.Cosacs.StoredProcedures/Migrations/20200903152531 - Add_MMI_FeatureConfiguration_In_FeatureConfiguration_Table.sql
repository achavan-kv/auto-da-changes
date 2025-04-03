-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- *************************************************************************************************
-- Developer:	Amit
-- Date:		20 August 2020
-- Purpose:		Add record for MMI configuration in FeatureConfiguration table.
-- *************************************************************************************************



GO

IF NOT EXISTS(SELECT 1 FROM [dbo].[FeatureConfiguration] WHERE FeatureName ='MMI')
BEGIN
		INSERT INTO [dbo].[FeatureConfiguration] (Id, FeatureName, ActivationDate)
		VALUES (1, 'MMI', GETDATE())
END

GO