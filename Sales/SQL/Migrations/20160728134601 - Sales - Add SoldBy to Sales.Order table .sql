-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Sales.[Order]
ADD SoldBy int NULL

GO

Update o 
Set o.SoldBy = o.CreatedBy
from Sales.[Order] o
GO

ALTER TABLE Sales.[Order]
ALTER COLUMN SoldBy int NOT NULL