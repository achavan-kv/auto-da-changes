-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @countrycode CHAR(1)

select @countrycode=countrycode from dbo.country

-- pk_delivery has been created as an index NOT primary key on Trinidad - causes problem when following migration tries to drop constraint
if @countrycode='T'			
begin
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[delivery]') AND name = N'pk_delivery')
DROP INDEX [pk_delivery] ON [dbo].[delivery] WITH ( ONLINE = OFF )

end
GO

select [acctno] ,
	[agrmtno] ,
	[itemID] ,
	[stocklocn] ,
	[buffno] ,
	[contractno] ,
	[ParentItemID], COUNT(*) as Nbr
into #dupdels
from delivery where itemid!=0
group by [acctno] ,
	[agrmtno] ,
	[itemID] ,
	[stocklocn] ,
	[buffno] ,
	[contractno] ,
	[ParentItemID] having COUNT(*)>1
	
select d.acctno,d.agrmtno,d.itemno,d.stocklocn,d.buffno,d.contractno,d.parentitemno,d.datetrans
into #dupdels2
 from delivery d INNER JOIN #dupdels s on d.acctno=s.acctno and d.agrmtno=s.agrmtno and d.itemid=s.itemid and d.stocklocn=s.stocklocn 
					and d.buffno=s.buffno and d.contractno=s.contractno and d.parentitemid=s.ParentItemID
					
alter TABLE #dupdels2 add  id INT IDENTITY

UPDATE delivery
	set buffno=d.buffno+ID
from delivery d INNER JOIN #dupdels2 s on d.acctno=s.acctno and d.agrmtno=s.agrmtno and d.itemno=s.itemno and d.stocklocn=s.stocklocn 
					and d.buffno=s.buffno and d.contractno=s.contractno and d.parentitemno=s.ParentItemno
where d.datetrans=s.datetrans

