-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from admin.Permission where id >= 2110 and id <= 2119

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2110, 'View Supplier', 'Allows user to view the supplier screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2111, 'Edit Supplier', 'Allows user to create and edit suppliers')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2112, 'View Regular Stock', 'Allows user to view the regular stock screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2113, 'Edit Regular Stock', 'Allows user to create and edit regular stock')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2114, 'View Repossessed Stock', 'Allows user to view the repossessed stock screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2115, 'Edit Repossessed Stock', 'Allows user to create and edit repossessed stock')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2116, 'View Products Without Stock', 'Allows user to view the products without stock screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2117, 'Edit Products Without Stock', 'Allows user to create and edit products without stock')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2118, 'View Spare Parts', 'Allows user to view the spare parts screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2119, 'Edit Spare Parts', 'Allows user to create and edit spare parts')

 --select * from admin.Permission