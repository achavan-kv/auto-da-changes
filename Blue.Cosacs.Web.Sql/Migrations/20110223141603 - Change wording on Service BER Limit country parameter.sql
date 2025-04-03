-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CountryMaintenance
	set Description='The percentage of the original Cost Price that when exceeded means the product is probably beyond economic repair (BER). Default will be 75%'
where codename='ServiceBER'
