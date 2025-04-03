-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Create Table Warehouse.DriverPayment
(
Id int identity(1,1),
SendingBranch	smallint NOT NULL,
ReceivingBranch	smallint NOT NULL,
Size varchar(100) NOT NULL,
Value decimal(15,4) NOT NULL
)
