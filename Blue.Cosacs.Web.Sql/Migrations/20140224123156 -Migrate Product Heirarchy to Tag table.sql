-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete Warranty.WarrantyReturnFilter
delete Warranty.WarrantyTags
delete warranty.[Level]


if not exists (select * from  Warranty.[Level] where name='Department')
insert into Warranty.[Level] (Name)
select 'Department'
--if not exists (select * from  Warranty.[Level] where name='PCF')
--select 'PCF'

if not exists (select * from  Warranty.tag where name='Electrical')
insert into warranty.tag (LevelId, Name)
select l.id,'Electrical'
from  Warranty.[Level] l where name='Department'

if not exists (select * from  Warranty.tag where name='Furniture')
insert into warranty.tag (LevelId, Name)
select l.id,'Furniture'
from  Warranty.[Level] l where name='Department'

--select l.id,h.CodeDescription
--from ProductHeirarchy h, warranty.level l 
--where CatalogType=02 and ParentCode in('7','8') 
--and l.Name=case when h.ParentCode='7' then 'PCF' else 'PCE' end
--and h.PrimaryCode<82 and PrimaryCode not in (12,82)
--and not exists( select * from warranty.tag where h.CodeDescription=Name)
--order by l.id,cast(primarycode as int)



insert into warranty.WarrantyTags (WarrantyId, TagId, LevelId)
select distinct w.id,t.id,l.id
from warranty.warranty w,warranty.level l,warranty.tag t
where w.Number like '19%'
and l.Name='Department'
and t.Name='Electrical' and t.LevelId=l.Id
and not exists( select * from warranty.WarrantyTags s where w.id=s.WarrantyId and s.LevelId=l.Id)
--group by w.id,t.id,l.id

--insert into warranty.WarrantyTags (WarrantyId, TagId, LevelId)
--select distinct w.id,t.id,l.id
--from warranty.warranty w,warranty.level l,warranty.tag t
--where w.Number like '19%'
--and l.Name='Furniture'
--and t.Name='NONE' and t.LevelId=l.Id
--and not exists( select * from warranty.WarrantyTags s where w.id=s.WarrantyId and s.LevelId=l.Id)


insert into warranty.WarrantyTags (WarrantyId, TagId, LevelId)
select distinct w.id,t.id,l.id
from warranty.warranty w,warranty.level l,warranty.tag t
where w.Number like 'XW%'
and l.Name='Department'
and t.Name='Furniture' and t.LevelId=l.Id
and not exists( select * from warranty.WarrantyTags s where w.id=s.WarrantyId and s.LevelId=l.Id)

--insert into warranty.WarrantyTags (WarrantyId, TagId, LevelId)
--select w.id,t.id,l.id
--from warranty.warranty w,warranty.level l,warranty.tag t
--where w.Number like 'XW%'
--and l.Name='Electrical'
--and t.Name='None' and t.LevelId=l.Id
--and not exists( select * from warranty.WarrantyTags s where w.id=s.WarrantyId and s.LevelId=l.Id)

