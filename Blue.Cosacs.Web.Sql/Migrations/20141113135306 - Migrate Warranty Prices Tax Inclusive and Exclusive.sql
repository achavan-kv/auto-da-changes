-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


     declare @taxrate float, @stockTaxType char(1)

     select @taxrate = value from CountryMaintenance where CodeName = 'TaxRate'
     select @stockTaxType = value from CountryMaintenance where CodeName = 'taxtype'

    --Do not delete entries for Free warranties as these will not change due to a 0 price.
    delete from 
        warranty.warrantyprice
    where exists
        (select * from warranty.warranty w
         where w.id = warranty.warrantyprice.WarrantyId
         and w.TypeCode != 'F')

    select distinct Number as waritemno
    into #migwar
    from Warranty.Warranty
	
	-- prices
	select 
        itemno,unitpricehp,unitpricecash,costprice,count(*) as nbr
	into 
        #migprices
	from 
        stockitem s 
    inner join 
        #migwar m on s.itemno=m.waritemno
	where 
        category in (12,82)
	group by
        itemno,unitpricehp,unitpricecash,costprice 
	order by 
        itemno,count(*) desc

	-- select items with only one price
	select 
        itemno,count(*)  as nbr
	into 
        #oneprice
	from 
        #migprices
	group by 
        itemno having count(*)=1

	select 
        itemno,nbr  
	into 
        #multiprice
	from 
        #migprices m
	where 
        not exists (select * from #oneprice o where o.itemno=m.itemno)

	insert into 
        Warranty.WarrantyPrice (WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
	select w.Id,
           null,
           null,
           isnull(p.costprice,0),
           case 
                when @stockTaxType = 'I' 
                    then isnull(p.unitpricecash,0) - (isnull(p.unitpricecash,0) * @taxrate) / (100 + @taxrate)
                else 
                    isnull(p.unitpricecash,0)
            end,
           getdate()
	from 
        Warranty.Warranty w 
    inner join 
        #oneprice o on w.Number=o.itemno
    inner join 
        #migprices p on o.itemno=p.itemno

	 -- add main price - (not branch specific)
	;with mainprice as (select itemno,max(nbr) as nbr from #multiprice group by itemno)

	insert into 
        Warranty.WarrantyPrice (WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
	select 
        w.Id,
        null,
        null,
        isnull(p.costprice,0),
        case 
            when @stockTaxType = 'I' 
                then isnull(p.unitpricecash,0) - (isnull(p.unitpricecash,0) * @taxrate) / (100 + @taxrate)
            else 
                isnull(p.unitpricecash,0)
        end,
        getdate()
	from 
        Warranty.Warranty w 
    inner join 
        #migprices p on w.Number=p.itemno 
	inner join 
        mainprice a on p.itemno=a.itemno 
        and a.nbr=p.nbr

	 -- add alternate price - (branch specific)
	;with altprice as (select itemno,min(nbr) as nbr from #multiprice group by itemno)

	insert into 
        Warranty.WarrantyPrice (WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice, EffectiveDate)
	select 
        w.Id,
        null,
        s.stocklocn,
        isnull(p.costprice,0),
        case 
            when @stockTaxType = 'I' 
                then isnull(p.unitpricecash,0) - (isnull(p.unitpricecash,0) * @taxrate) / (100 + @taxrate)
            else 
                isnull(p.unitpricecash,0)
        end,
        getdate()
	from 
        Warranty.Warranty w 
    inner join 
        #migprices p on w.Number=p.itemno 
    inner join 
        altprice a on p.itemno=a.itemno 
        and a.nbr=p.nbr
    inner join 
        stockitem s on a.itemno=s.itemno 
        and isnull(p.costprice,0)=isnull(s.CostPrice,0) 
        and p.unitpricecash=s.unitpricecash
	inner join 
        branch b on s.stocklocn = b.branchno


	



