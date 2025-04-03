-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issues: #11131, #13333


IF EXISTS(select * from admin.Permission where name = 'Supplier Cost Matrix')
BEGIN
	UPDATE admin.Permission SET Name = 'Supplier Contractual Costs', Description = 'Allows access to the Supplier Contractual Costs screen'
	WHERE name = 'Supplier Cost Matrix'
END


IF EXISTS(select * from admin.Permission where name = 'Authorise Delivery - Cash accounts')
BEGIN
	UPDATE admin.Permission set Description = 'Authorise Delivery - Makes the Include Cash check box available on the Authorise Delivery Screen'
	WHERE name = 'Authorise Delivery - Cash accounts'
END