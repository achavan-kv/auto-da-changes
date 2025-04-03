-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
if (select count(*) from sys.columns c join sys.tables t
on c.object_id = t.object_id
where t.name = '$Migration' and c.name = 'Checksum') = 0
begin

   ALTER TABLE dbo.[$Migration] ADD [Checksum] varchar(100) NULL

end
GO