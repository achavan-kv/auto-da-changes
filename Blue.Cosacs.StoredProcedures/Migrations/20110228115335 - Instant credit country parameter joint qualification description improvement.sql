-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CountryMaintenance 
SET Description = 'If this option is on, accounts where the customer is a joint holder must also qualify, for the customer to qualify'
WHERE  codename = 'IC_JointQualification'