-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12508

DELETE 
FROM dbo.CountryMaintenance 
WHERE CodeName IN ('ServiceBER','ServiceReplacement', 'ServiceInternal',  'ServiceItemLabour', 'ServiceItemPartsCourts', 'ServiceItemPartsOther',
														'ServiceLabourRate', 'ServiceLabourMarkUp', 'ServicePartsMarkUp', 'ServiceStockAccount', 'ServiceWarranty', 'InstalChgAcct')