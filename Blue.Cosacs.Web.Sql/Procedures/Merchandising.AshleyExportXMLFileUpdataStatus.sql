 if exists ( select *  from  dbo.sysobjects   where  id = object_id('[Merchandising].[AshleyExportXMLFileUpdataStatus]')    and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure  [Merchandising].[AshleyExportXMLFileUpdataStatus]
GO  
create PROCEDURE  [Merchandising].[AshleyExportXMLFileUpdataStatus]
as 
Begin
declare @id varchar(50)='AshleyEnabled'
If @id=(select Id from config.setting where Id='AshleyEnabled' and ValueBit=1 )
DECLARE @PoNumber int
SET @PoNumber = (SELECT DISTINCT TOP 1
  PoNumber
FROM  [Merchandising].[XMLFILEDataNew]
WHERE FileCreateStatus = 0)
SELECT TOP 1
  a.PoNumber,
  o.XML
FROM (SELECT
  MAX(RowNo) AS LastXMLNo,
  PoNumber
FROM [Merchandising].[XMLFILEDataNew] t
WHERE FileCreateStatus = 0
GROUP BY PoNumber) a
INNER JOIN [Merchandising].[XMLFILEDataNew] o
  ON a.PoNumber = o.PoNumber
  AND a.LastXMLNo = o.RowNo
WHERE a.PoNumber = @PoNumber and FileCreateStatus=0
UPDATE [Merchandising].[XMLFILEDataNew]
SET FileCreateStatus = 1
WHERE PoNumber = @PoNumber
print 'Ashley product not configured on this country'
end 