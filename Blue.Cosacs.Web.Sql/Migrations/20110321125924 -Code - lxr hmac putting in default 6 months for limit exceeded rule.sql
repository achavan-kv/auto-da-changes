-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE code SET sortorder = 6 -- this is storing minimum agreement length for this code and category
WHERE code LIKE 'hmAc%' AND CATEGORY ='LXR'
