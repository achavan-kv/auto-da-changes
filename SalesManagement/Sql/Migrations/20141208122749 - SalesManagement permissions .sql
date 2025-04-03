-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


INSERT INTO Admin.PermissionCategory
VALUES(24, 'Sales Management')

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2400,
				'CSR Call Log',
				24,
				'Allow user to search for calls. Allow user to Log a call and to log an unscheduled call.'
          ),
		  (
				2401,
				'Configure Follow up Calls',
				24,
				'Allow user to configure the follow up calls, add and delete a configuration.'
		   ),
		   (
				2402,
				'Branch Manager Call List',
				24,
				'Allow user to search for calls and to allocate unallocated calls.'
		   ),
		   (
				2403,
				'CSR Unavailable',
				24,
				'Allow user to search for sales person and to make the CSR unavailable. Allow user to add and delete sales person unavailability.'
		   ),
		   (
				2404,
				'Quick Details Capture',
				24,
				'Allow user to search for an existing customer. Allow user to schedule a call for an existing customer or a new one.'
		   ),
		   (
				2405,
				'CSR Targets',
				24,
				'Allow user to configure their own sales amount targets.'
		   ),
		   (
				2406,
				'Customer search - My Customers',
				24,
				'Allow user to search for customers and to schedule calls for them.'
		   ),
		    (
				2407,
				'Customer search - Branch Customers',
				24,
				'Allow user to search for customers and to schedule calls for them.'
		   ),
		    (
				2408,
				'Customer search - Reallocate Customers',
				24,
				'Allow user to search for customers and to reallocate them to another CSR.'
		   ),
		   (
				2409,
				'Call Icon',
				24,
				'Allow user to select an icon for call types.'
		   )
