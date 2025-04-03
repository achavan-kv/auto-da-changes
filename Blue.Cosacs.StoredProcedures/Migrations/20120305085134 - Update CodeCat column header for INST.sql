-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from CodeCat where category = 'INST')
BEGIN
	update CodeCat
	set ReferenceHeaderText = 'Category'
	where Category = 'INST'
END