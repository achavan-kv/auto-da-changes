-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into [admin].[permission] (Id,Name,CategoryId,Description)
Values(1433, 'Internal Driver Payments View', 14, 'View the setup of driver commission for internal deliveries between stores')
,(1434, 'Internal Driver Payments Edit', 14, 'Edit the setup of driver commission for internal deliveries between stores')
