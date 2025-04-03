-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'Level_1' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyReturn'))
BEGIN
    ALTER TABLE Warranty.WarrantyReturn ADD [Level_1] [varchar](100) NULL
END

go

--if not exists (select * from  warranty.tag t inner join Warranty.[Level] l on t.LevelId=l.Id where l.Name='PCE' and t.Name='ANY')
--insert into warranty.tag (LevelId, Name)
--select l.id,'ANY' from Warranty.[Level] l  where l.Name='PCE'

--if not exists (select * from  warranty.tag t inner join Warranty.[Level] l on t.LevelId=l.Id where l.Name='PCF' and t.Name='ANY')
--insert into warranty.tag (LevelId, Name)
--select l.id,'ANY' from Warranty.[Level] l  where l.Name='PCF'

delete warranty.WarrantyReturnFilter
delete Warranty.WarrantyReturn

-- Electrical prices
	insert into  Warranty.WarrantyReturn (WarrantyLength, ElapsedMonths, PercentageReturn, BranchType, BranchNumber, WarrantyId,[Level_1])
	select distinct WarrantyMonths,MonthSinceDelivery,refundpercentfromAIG,null,null,null,'PCE'
	from WarrantyReturnCodes 
	where producttype='E'

	insert into Warranty.WarrantyReturnFilter (WarrantyReturnId, TagId)
	select r.id,t.Id
	from  Warranty.WarrantyReturn r, warranty.[Level] l inner join Warranty.tag t on l.id=t.LevelId 
	where ( (l.Name='Department' and t.Name='Electrical' and r.WarrantyId is null and r.level_1='PCE')
		--or (l.Name='Furniture' and t.Name='NONE' and r.WarrantyId is null) 
		)
		and not exists( select * from Warranty.WarrantyReturnFilter f where f.WarrantyReturnId=r.id and f.tagid = t.id)


-- Furniture prices
	insert into  Warranty.WarrantyReturn (WarrantyLength, ElapsedMonths, PercentageReturn, BranchType, BranchNumber, WarrantyId,[Level_1])
	select distinct WarrantyMonths,MonthSinceDelivery,refundpercentfromAIG,null,null,null,'PCF'
	from WarrantyReturnCodes 
	where producttype='F'

	insert into Warranty.WarrantyReturnFilter (WarrantyReturnId, TagId)
	select r.id,t.Id
	from  Warranty.WarrantyReturn r, warranty.[Level] l inner join Warranty.tag t on l.id=t.LevelId 
	where ( (l.Name='Department' and t.Name='Furniture' and r.WarrantyId is null and r.level_1='PCF')
		--or (l.Name='Furniture' and t.Name='ANY' and r.WarrantyId is null) 
			)
	--	and not exists( select * from Warranty.WarrantyReturnFilter f where f.WarrantyReturnId=r.id and (t.Name='ANY' or t.Name='NONE'))

