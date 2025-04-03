-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update countrymaintenance
set description = 'Number of months back to check if account has been referred. If account has been referred within ''x'' months, it will not qualify for instant credit'
where codename = 'IC_ReferralMonths'