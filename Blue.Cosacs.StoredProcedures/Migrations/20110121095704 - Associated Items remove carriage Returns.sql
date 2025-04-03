-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--remove carriage return, line feed, and tab 
UPDATE dbo.stockitemassociated
SET associateditemcategory=REPLACE(REPLACE(REPLACE(associateditemcategory, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')
