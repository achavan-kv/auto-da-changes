-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CountryMaintenance
	set Description='This is the name shown in service request when selecting the Service Type, e.g. if "CoSACS"; options are "CoSACS Account", "Non CoSACS". A maximum of 7 characters can be entered.'
Where codename='SRAcctName'
	
