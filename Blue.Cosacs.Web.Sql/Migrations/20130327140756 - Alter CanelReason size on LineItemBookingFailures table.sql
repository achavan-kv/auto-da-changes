-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12851


IF EXISTS(select * from syscolumns where name = 'CancelReason' and object_name(id) = 'LineItemBookingFailures')
BEGIN
	ALTER TABLE LineItemBookingFailures ALTER COLUMN CancelReason varchar(200) null
END	
