-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM  StoreCardStatus_Lookup WHERE Status = 'C')
INSERT INTO StoreCardStatus_Lookup (
	Status,
	Description
) VALUES ( 
	/* Status - varchar(5) */ 'C',
	/* Description - varchar(50) */ 'Cancelled' ) 