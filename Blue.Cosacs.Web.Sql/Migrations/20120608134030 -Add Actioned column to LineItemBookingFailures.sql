-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Actioned'
               AND OBJECT_NAME(id) = 'LineItemBookingFailures')
BEGIN
  ALTER TABLE LineItemBookingFailures ADD Actioned int 
END

