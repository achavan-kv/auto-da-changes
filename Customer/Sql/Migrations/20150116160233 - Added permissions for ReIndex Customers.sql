-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


INSERT INTO Admin.PermissionCategory
VALUES(26, 'Customer')

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2600,
				'Re-index customer',
				26,
				'Allow user to re-index customers'
          )