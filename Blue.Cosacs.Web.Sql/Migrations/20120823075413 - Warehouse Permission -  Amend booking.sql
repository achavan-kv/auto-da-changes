-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

exec Admin.AddPermission 1428, 'Amend Booking', 14, 'Allows user to Amend or Cancel Bookings and Exceptions'
