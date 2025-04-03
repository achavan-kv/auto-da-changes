-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE dbo.delivery
	set RunNo=-999 
	Where RunNo=0 and datetrans <'2011-01-01'