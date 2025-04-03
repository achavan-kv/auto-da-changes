-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from admin.Permission where id >= 2110 and id <= 2120

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2110, 'Supplier View', 'Allows user to view the supplier screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2111, 'Supplier Edit', 'Allows user to create and edit suppliers')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2112, 'Stock View', 'Allows user to view stock screens')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2113, 'Regular Stock Edit', 'Allows user to create and edit regular stock')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2114, 'Repossessed Stock Edit', 'Allows user to create and edit repossessed stock')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2115, 'Products Without Stock Edit', 'Allows user to create and edit products without stock')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2116, 'Spare Parts Edit', 'Allows user to create and edit spare parts')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2117, 'Sets View', 'Allows user view the sets screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2118, 'Sets Edit', 'Allows user create and edit sets')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2119, 'Combo View', 'Allows user view the combo screen')

INSERT INTO [Admin].[Permission](CategoryId, Id, Name, [Description])
VALUES (21, 2120, 'Combo Edit', 'Allows user create and edit combos')