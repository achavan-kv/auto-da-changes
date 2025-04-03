-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into Admin.Permission
(CategoryId, Id, Name, Description)
VALUES
(21, 2125, 'TaxRateView', 'Allows user view the tax rate maintenance screen'),
(21, 2126, 'TaxRateEdit', 'Allows user create and edit system tax rates')