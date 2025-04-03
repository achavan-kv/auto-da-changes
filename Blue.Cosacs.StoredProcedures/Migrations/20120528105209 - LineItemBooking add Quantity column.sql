-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Quantity'
               AND OBJECT_NAME(id) = 'LineItemBooking')
BEGIN
  ALTER TABLE LineItemBooking ADD Quantity float not null default 0
END