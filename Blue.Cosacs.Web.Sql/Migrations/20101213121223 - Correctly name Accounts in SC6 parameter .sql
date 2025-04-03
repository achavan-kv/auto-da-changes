-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE countryMaintenance
	set Name='A: Months Account in Status 6',[Description]='Automatic: Number of months an account has been in SC6 (Default 3 months) before it will be generated for Write Off in the Write Off Review screen.'
where codename='mthsstatus'