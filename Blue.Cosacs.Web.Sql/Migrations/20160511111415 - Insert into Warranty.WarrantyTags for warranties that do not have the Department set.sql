-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into Warranty.WarrantyTags (WarrantyId, TagId, LevelId)
select 
    w.Id,
    t.Id,
    t.LevelId
from
    Warranty.Warranty w,
    Warranty.Tag t
where
    t.Name = 'Electrical'
    and not exists(select * from Warranty.WarrantyTags wt
                    where wt.WarrantyId = w.Id)
    and w.Number like '19%'
union
select 
    w.Id,
    t.Id,
    t.LevelId
from
    Warranty.Warranty w,
    Warranty.Tag t
where
    t.Name = 'Furniture'
    and not exists(select * from Warranty.WarrantyTags wt
                    where wt.WarrantyId = w.Id)
    and w.Number like 'XW%'
