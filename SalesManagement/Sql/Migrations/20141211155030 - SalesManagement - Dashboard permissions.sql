-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2410,
				'CSR Dashboard',
				24,
				'Allow user to see CSR Dashboard.'
          ),
		  (
				2411,
				'Branch Manager Dashboard',
				24,
				'Allow user to see the Branch Manager Dashboard'
		   )
		 