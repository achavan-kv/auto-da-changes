-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE cmstrategycondition
SET operator1 = 50,
	operand = '<'
WHERE strategy = 'SCCOL' 
AND condition = 'ArrearsPCSC'
AND SavedType = 'X'
