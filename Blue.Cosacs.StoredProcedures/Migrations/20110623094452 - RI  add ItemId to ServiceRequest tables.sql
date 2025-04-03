-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'PartID'
               AND OBJECT_NAME(id) = 'SR_PartListResolved')
BEGIN
  ALTER TABLE SR_PartListResolved ADD PartID INT not null default 0
END


go

UPDATE SR_PartListResolved
	set PartID=i.id
from SR_PartListResolved p INNER JOIN Stockinfo i on i.itemno=p.partno
where ISNULL(p.PartID,0)=0 

go

alter TABLE SR_ServiceRequest alter column ProductCode VARCHAR(18) not null
go

