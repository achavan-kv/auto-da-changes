-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE code SET statusflag = 'L'
WHERE category = 'STCM'

UPDATE code SET  sortorder = 1 WHERE category ='STCM' AND code = 'email'