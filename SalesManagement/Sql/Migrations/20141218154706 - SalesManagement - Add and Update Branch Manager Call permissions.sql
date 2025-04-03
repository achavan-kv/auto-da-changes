-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2412,
				'Branch Manager - Unallocated Calls',
				24,
				'Allow user to allocate unallocated calls'
          )

GO

UPDATE Admin.Permission
Set [Name] = 'Branch Manager - Reallocate Calls', 
	Description = 'Allow user to search for calls and to reallocate calls.'
WHERE Id = 2402