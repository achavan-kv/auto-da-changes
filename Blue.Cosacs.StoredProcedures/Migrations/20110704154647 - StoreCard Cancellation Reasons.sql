-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DELETE FROM code WHERE category = 'STCR' AND code = 'stol' AND codedescript LIKE 'canc%'

UPDATE codecat SET ReferenceHeaderText = 'Recontact Months' WHERE category='STC'

UPDATE codecat SET catdescript = 'Store Card Account Cancellation' WHERE  category= 'STC'