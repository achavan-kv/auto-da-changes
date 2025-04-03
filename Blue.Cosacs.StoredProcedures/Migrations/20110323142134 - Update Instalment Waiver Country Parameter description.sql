-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from countrymaintenance where codename = 'InstalWaiverMinScore')
BEGIN
	UPDATE countrymaintenance
	SET name = 'Min score for delivery without first instalment',
		[description] = 'This is the minimum score needed for an account to qualify for delivery without first instalment. If this parameter is set to zero,delivery without first instalment is not enabled'
	WHERE codename = 'InstalWaiverMinScore'
END