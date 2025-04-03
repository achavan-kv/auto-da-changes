-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12851


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Cancellation' AND  Column_Name = 'Reason'
           AND TABLE_SCHEMA = 'Warehouse')
BEGIN

	ALTER TABLE Warehouse.Cancellation ALTER COLUMN Reason varchar(200) null

END