 if exists ( select *  from  dbo.sysobjects   where  id = object_id('[ReadXMLFileDataandUpdate]')    and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure  [ReadXMLFileDataandUpdate]
go
 Create PROCEDURE [dbo].[ReadXMLFileDataandUpdate]
AS
BEGIN
declare @id1 varchar(50)='AshleyEnabled'
If @id1=(select Id from config.setting where Id='AshleyEnabled' and ValueBit=1 )

with  CTE as ( select Row_Number()over(Partition by FileName order by FileName asc)   RowNumber  from XMLFilesTable where  ReadStatus=0 )
 delete CTE WHERE RowNumber > 1  
 
IF OBJECT_ID('tempdb..#tt_ReplaceXML') IS NOT NULL    DROP TABLE #tt_ReplaceXML  
IF OBJECT_ID('tempdb..#ImportedXML') IS NOT NULL    DROP TABLE #ImportedXML  
 
--select * from XMLFilesTable
select CONVERT(varchar(5000), XMLData) as NewConvert,FileName,ReadStatus,Id  into #ImportedXML from XMLFilesTable where ReadStatus=0
select convert(xml,REPLACE(REPLACE(NewConvert,'fnPOAck:',''),'fnParty:',''))  as NewXMLNew,FileName,ReadStatus,id into #tt_ReplaceXML   from #ImportedXML t

BEGIN TRAN
DECLARE @site_value INT;
DECLARE @site_value1 int =1;
Select @site_value=count(*)  from #tt_ReplaceXML 
WHILE  @site_value>=@site_value1
BEGIN
DECLARE @x xml   
declare @id int
declare @PoNumber int
Declare @creationDate Date
Declare @Satus varchar(50)
--drop table #update 
set @id = (select top 1 id from #tt_ReplaceXML where ReadStatus=0 order  by id  desc  )
SET @x= (select Top 1 NewXMLNEw from #tt_ReplaceXML where ReadStatus=0 order  by id  desc )
Select @PoNumber=(SELECT x.XmlCol.value('(/poAck/ackOrder/orderDocument/@id)[1]', 'int') as [PoNumber]  FROM  @x.nodes('/poAck') x(XmlCol))
 Print @PoNumber 
Select @creationDate=(select x.XmlCol.value('(/poAck/ackOrder/ackDocument/creationDate)[1]', 'Date') as creationDate FROM  @x.nodes('/poAck') x(XmlCol))
 print @creationDate
Select @Satus=(select x.XmlCol.value('(/poAck/ackOrder/actionRequestIndicator/@description)[1]', 'varchar(50)') as Status  FROM  @x.nodes('/poAck') x(XmlCol))
print @Satus
update #tt_ReplaceXML set ReadStatus=1  where  id=@id
update XMLFilesTable set  ReadStatus=1,CreationDate=@creationDate,PoNumber=@PoNumber,actionRequestIndicator=@Satus   where  id=@id
  set @site_value1=@site_value1+1
--print @site_value1
END
Commit
 
update o set RequestedDeliveryDate=t.CreationDate,Status=actionRequestIndicator 
from XMLFilesTable t inner join [Merchandising].[PurchaseOrder] o on t.PoNumber=o.id and t.actionRequestIndicator<>o.Status
 --delete XMLFilesTable
--select * from XMLFilesTable
--select * from [Merchandising].[PurchaseOrder] where id in (select PoNumber from XMLFilesTable)
--end
--update  [Merchandising].[PurchaseOrder]  set Status='UnAproved' where id in (select PoNumber from XMLFilesTable)
  --select Row_Number()over(Partition by FileName order by FileName asc)   RowNumber,* into #Temp1 from XMLFilesTable 
  --delete  XMLFilesTable where id in (select id from #Temp1 where RowNumber>1 )

  --select * from XMLFilesTable where id not in (select fileName from #Temp1 where RowNumber>1 or ReadStatus=0 )

  --select * from #Temp1
 
 print 'Ashley product not configured on this country'
end 