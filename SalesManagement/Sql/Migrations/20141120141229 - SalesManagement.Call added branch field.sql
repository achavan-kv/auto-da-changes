-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- This field will be used for the new 'customers' that we add in the Quick Details Capture screen.

ALTER TABLE SalesManagement.Call
ADD Branch smallint NULL 
