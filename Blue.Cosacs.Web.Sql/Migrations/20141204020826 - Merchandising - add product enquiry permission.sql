-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into [admin].[Permission]
(id, name, categoryid, [description])
values(2141, 'Product Enquiry', 21, 'Allows access to the Product Enquiry Screen')
