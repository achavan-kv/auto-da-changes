SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].NonStockGetDetailsSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE NonStockGetDetailsSP
END
GO

CREATE PROCEDURE dbo.NonStockGetDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : NonStockGetDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Non Stock Item details
-- Author       : John Croft
-- Date         : 08 December 2010
--
-- This procedure will get the Non Stock Item details
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 01/02/11  jec NonStock changes
-- 25/05/11  jec CR1212 RI Integration
-- 16/01/12  ip  #9449 - Bracket was in incorrect position
-- ================================================
	-- Add the parameters for the stored procedure here
	@itemNo VARCHAR(8),
	@return INT output

As
	set @return=0
	
	select distinct s.IUPC as itemNo,itemdescr1,itemdescr2,s.category as 'Category', supplier as Supplier,suppliercode as SupplierCode,
		prodstatus, Deleted,taxrate as 'TaxRate',		
		itemtype as 'ItemType',c.category as ProdGroup,value as 'TaxType', 
		--,case when cw.category is null then 'N' else 'Y' end as Warranty
		ISNULL(s.DateActivated,'1900-01-01') as StartDate,ISNULL(DeletionDate,'1900-01-01') as EndDate,
		s.ItemID				-- RI
	from stockitem s INNER JOIN code c on CAST(s.category as VARCHAR(3)) = c.code 
						and c.category in('PCE','PCF','PCO','PCW','PCDIS')				
				LEFT OUTER JOIN nonstockdeletiondates d on s.itemno=d.itemno,
		countrymaintenance cm	
	where s.itemtype='N'
		and s.category not in (select code from code c2 where c2.category='WAR')
		and cm.CodeName='TaxType'
		and s.IUPC not like '19%'		-- like warranty but not in warranty category
		--and itemtype='N'	-- nonstock
		--and s.category not in(select code from code where category='WAR')
	order by s.IUPC				-- RI
		
 --price details
select i.IUPC as 'ItemNo',			-- RI
		p.BranchNo,CAST(ISNULL(CreditPrice,0) as DECIMAL(11,2)) as unitpriceHP,
		CAST(ISNULL(CashPrice,0) as DECIMAL(11,2)) as unitpricecash,
		CAST(ISNULL(DutyFreePrice,0) as DECIMAL(11,2)) as unitpricedutyfree,
		CAST(ISNULL(costprice,0) as DECIMAL(11,2)) as costprice,
		b.StoreType,i.ID as ItemID				-- RI 
from stockprice p INNER JOIN stockinfo i on p.ID = i.ID			-- RI p.itemno = i.itemno
					INNER JOIN code c on CAST(i.category as VARCHAR(3)) = c.code 
						and c.category in('PCE','PCF','PCO','PCW','PCDIS')	
					INNER JOIN Branch b on b.branchno=p.branchno					
where itemtype='N' and i.category not in (select code from code where category='WAR')
	and i.IUPC not like '19%'		-- like warranty but not in warranty category

--Branches details
select Branchno,storetype from branch order by Branchno desc

-- Stock/Warranty Details

select distinct IUPC as itemno,ID as ItemID				-- RI
from stockinfo i 
where (itemtype!='N' or i.category in (select code from code where category='WAR'))			-- IP - 16/01/12 - #9449
	and i.IUPC not like '19%'		-- like warranty but not in warranty category
 	
	set @return=@@ERROR

go	
-- end end end end end end end end end end end end end end end end end end end end end end end 