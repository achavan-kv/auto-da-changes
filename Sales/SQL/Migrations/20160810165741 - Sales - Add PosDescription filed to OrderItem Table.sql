-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'PosDescription' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
			  	  
	ALTER TABLE [Sales].[OrderItem]
	ADD PosDescription varchar(128) NULL
END
GO