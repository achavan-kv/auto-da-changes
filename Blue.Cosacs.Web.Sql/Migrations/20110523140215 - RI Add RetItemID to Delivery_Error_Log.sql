-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RetItemID'
               AND OBJECT_NAME(id) = 'Delivery_Error_log')
BEGIN
  ALTER TABLE Delivery_Error_log ADD RetItemID INT not null default 0
END
go

UPDATE Delivery_Error_log
	set RetItemID=i.ID 
From Delivery_Error_log d INNER JOIN Stockinfo i on i.itemno=d.RetItemNo
where ISNULL(d.RetItemID,0)=0 and d.RetItemNo!=''
go

