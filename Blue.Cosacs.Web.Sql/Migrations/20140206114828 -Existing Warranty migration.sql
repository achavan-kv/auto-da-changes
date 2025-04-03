-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from warranty.warranty w
			where exists(select * from warrantyband wb where w.Number = wb.waritemno))
BEGIN
	select distinct refcode,'Mig_Ref_' + refcode as LinkName
	into #migref_link
	from warrantyband
	where not exists(select * from warranty.Link where Name='Mig_Ref_' + refcode)

	-- Link
	insert into warranty.Link
	select LinkName,Getdate()
	from #migref_link

	-- LinkProduct
	insert into warranty.LinkProduct (LinkId, StockBranch, ItemNumber, Level_1, Level_2, Level_3, StoreType, RefCode)
	select wl.id,null,null,null,null,null,null,m.refcode
	from warranty.Link wl inner join #migref_link m on wl.Name=m.LinkName
	where not exists (select * from warranty.LinkProduct p where wl.id=p.linkid and p.refcode=m.refcode)

	select distinct waritemno,refcode
	into #migwar
	from warrantyband 
	union
	select distinct waritemno+'M',refcode
	from warrantyband 

	-- Warranties
	insert into Warranty.Warranty (Number, Description, Length, TaxRate, Free, Deleted)
	select distinct b.waritemno,left(i.itemdescr1,32),b.warrantylength-b.firstYearWarPeriod,i.taxrate,0,0
	from warrantyband b inner join stockinfo i on b.itemid=i.id
	where not exists(select * from warranty.Warranty w where b.waritemno=w.Number)	
	union
	select distinct b.waritemno+'M',left('Free '+ i.itemdescr1,32),b.firstYearWarPeriod,0,1,0
	from warrantyband b inner join stockinfo i on b.itemid=i.id
	where not exists(select * from warranty.Warranty w where b.waritemno+'M'=w.Number)	
	order by b.waritemno

	-- Link
	insert into Warranty.LinkWarranty (LinkId, WarrantyId, ProductMin, ProductMax)
	select l.Id,w.id,b.minprice,b.maxprice
	from warranty.Link l inner join #migref_link m on l.Name=m.LinkName		
			inner join warrantyband b on m.refcode=b.refcode
			inner join #migwar mw on mw.refcode=b.refcode and (b.waritemno= mw.waritemno or b.waritemno= mw.waritemno+'M')
			inner join Warranty.Warranty w on w.number=mw.waritemno or w.number= mw.waritemno+'M'
	where not exists(select * from Warranty.LinkWarranty lw where lw.LinkId=l.id and lw.WarrantyId=w.id)

	-- prices
	select itemno,unitpricehp,unitpricecash,costprice,count(*) as nbr
	into #migprices
	from stockitem s inner join #migwar m on s.itemno=m.waritemno
	where category in  (12,82)
	group by itemno,unitpricehp,unitpricecash,costprice 
	order by itemno,count(*) desc

	-- select items with only one price
	select itemno,count(*)  as nbr
	into #oneprice
	from #migprices
	group by itemno having count(*)=1

	select itemno,nbr  
	into #multiprice
	from #migprices m
	where not exists (select * from #oneprice o where o.itemno=m.itemno)
	--group by itemno having count(*)>1

	insert into Warranty.WarrantyPrice (WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
	select w.Id,null,null,isnull(p.costprice,0),isnull(p.unitpricecash,0),getdate()
	from Warranty.Warranty w inner join #oneprice o on w.Number=o.itemno
			inner join #migprices p on o.itemno=p.itemno

	 -- add main price - (not branch specific)
	;with mainprice as (select itemno,max(nbr) as nbr from #multiprice group by itemno)
	insert into Warranty.WarrantyPrice (WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
	select w.Id,null,null,isnull(p.costprice,0),isnull(p.unitpricecash,0),getdate()
	from Warranty.Warranty w inner join #migprices p on w.Number=p.itemno 
			inner join mainprice a on p.itemno=a.itemno and a.nbr=p.nbr

	 -- add alternate price - (branch specific)
	;with altprice as (select itemno,min(nbr) as nbr from #multiprice group by itemno)
	insert into Warranty.WarrantyPrice (WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
	select w.Id,null,s.stocklocn,isnull(p.costprice,0),isnull(p.unitpricecash,0),getdate()
	from Warranty.Warranty w inner join #migprices p on w.Number=p.itemno 
			inner join altprice a on p.itemno=a.itemno and a.nbr=p.nbr
			inner join stockitem s on a.itemno=s.itemno and isnull(p.costprice,0)=isnull(s.CostPrice,0) and p.unitpricecash=s.unitpricecash
			inner join branch b on s.stocklocn = b.branchno


	-- warranty Return Percentages
	-- Level
	if not exists(select * from warranty.[Level] where name='Electrical')
		insert into warranty.[Level] (Name) values('Electrical')
	if not exists(select * from warranty.[Level] where name='Furniture')
		insert into warranty.[Level] (Name) values('Furniture')

	-- Tag
	insert into Warranty.tag (LevelId, Name)
	select l.Id,'ANY'
	from warranty.[Level] l 
	where not exists(select * from warranty.Tag t inner join warranty.[Level] l on l.id=t.LevelId and t.Name ='ANY')

	insert into Warranty.tag (LevelId, Name)
	select l.Id,'NONE'
	from warranty.[Level] l 
	where not exists(select * from warranty.Tag t inner join warranty.[Level] l on l.id=t.LevelId and t.Name ='NONE')

	-- Electrical prices
	insert into  Warranty.WarrantyReturn (WarrantyLength, ElapsedMonths, PercentageReturn, BranchType, BranchNumber, WarrantyId)
	select distinct WarrantyMonths,MonthSinceDelivery,refundpercentfromAIG,null,null,null
	from WarrantyReturnCodes 
	where producttype='E'

	insert into Warranty.WarrantyReturnFilter (WarrantyReturnId, TagId)
	select r.id,t.Id
	from  Warranty.WarrantyReturn r, warranty.[Level] l inner join Warranty.tag t on l.id=t.LevelId 
	where ( (l.Name='Electrical' and t.Name='ANY' and r.WarrantyId is null)
		or (l.Name='Furniture' and t.Name='NONE' and r.WarrantyId is null) )
		and not exists( select * from Warranty.WarrantyReturnFilter f where f.WarrantyReturnId=r.id and f.tagid = t.id)

	-- Furniture prices
	insert into  Warranty.WarrantyReturn (WarrantyLength, ElapsedMonths, PercentageReturn, BranchType, BranchNumber, WarrantyId)
	select distinct WarrantyMonths,MonthSinceDelivery,refundpercentfromAIG,null,null,null
	from WarrantyReturnCodes 
	where producttype='F'

	insert into Warranty.WarrantyReturnFilter (WarrantyReturnId, TagId)
	select r.id,t.Id
	from  Warranty.WarrantyReturn r, warranty.[Level] l inner join Warranty.tag t on l.id=t.LevelId 
	where ( (l.Name='Electrical' and t.Name='NONE' and r.WarrantyId is null)
		or (l.Name='Furniture' and t.Name='ANY' and r.WarrantyId is null) )
		and not exists( select * from Warranty.WarrantyReturnFilter f where f.WarrantyReturnId=r.id and (t.Name='ANY' or t.Name='NONE'))

END
go



