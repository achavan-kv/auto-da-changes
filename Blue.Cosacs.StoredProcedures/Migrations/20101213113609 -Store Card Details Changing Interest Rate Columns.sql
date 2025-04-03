-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE storecardratedetails DROP COLUMN RetailRateFixed
ALTER TABLE storecardratedetails DROP COLUMN RetailRateVariable
GO 
ALTER TABLE storecardratedetails ADD PurchaseInterestRate interest_rate
ALTER TABLE storecardratedetails ADD ratefixed bit
