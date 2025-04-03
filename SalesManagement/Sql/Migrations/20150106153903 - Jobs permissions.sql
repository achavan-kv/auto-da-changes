-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2413,
				'Customer Instalment Ending Job',
				24,
				'Background job that schedule calls for customers that have paid their last instalment'
          ),
		  ( 
				2414,
				'Allocate Customers To CSR',
				24,
				'Background job that allocate customers to CSR'
          ),
		  ( 
				2415,
				'Inactive customers',
				24,
				'Background job that get the inactive customers'
          ),
		  ( 
				2416,
				'Flush Unmade Calls',
				24,
				'Background job that flushes the calls that were unmade for X days'
          ),
		  ( 
				2417,
				'Sales Follow Up Calls',
				24,
				'Background job that gets the follow up calls'
          ),
		  ( 
				2418,
				'Reload Dashboard',
				24,
				'Background job that reload the Dashboard'
          )
		  
		  
		  