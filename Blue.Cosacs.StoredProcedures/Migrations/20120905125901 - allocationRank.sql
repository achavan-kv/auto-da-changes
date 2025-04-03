if  exists (select * from sysobjects where name = 'trig_CourtsPerson_income')
    drop trigger trig_CourtsPerson_income
go


UPDATE dbo.CourtsPersonTable
SET AllocationRank = 5
WHERE AllocationRank = 0

ALTER TABLE dbo.CourtsPersonTable
ADD CONSTRAINT AllocationRankCheck CHECK (AllocationRank > 0)
