-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE SalesManagement.CallType
ADD IsSystemType bit NULL

Go

UPDATE SalesManagement.CallType
Set IsSystemType = 0

Go

ALTER TABLE SalesManagement.CallType
ALTER COLUMN IsSystemType bit NOT NULL

Go 

INSERT INTO SalesManagement.CallType
Values(5, 'Unscheduled Call', null, 1)

Go






