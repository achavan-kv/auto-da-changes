-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CodeCat 
	set ReferenceHeaderText='Default DN Branch'
where category in ('PCE','PCF','PCW')