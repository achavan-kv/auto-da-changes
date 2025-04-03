IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductAssociationGetDetailsSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ProductAssociationGetDetailsSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[ProductAssociationGetDetailsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : ProductAssociationGetDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get dropdow data
-- Date         : 09 June 2011
--
-- This procedure will get the Dropdown data for Product Associations
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
			@return INT out
As	
	set @return=0
		
	-- Dropdown details
	Select distinct ph.primaryCode as ProductGroup,ph.CodeDescription as ProductGroupDescr 
	from ProductHeirarchy ph 
	where ph.CatalogType = '01'		-- ProductGroup/Department
		and CodeStatus='A'			-- Active
		and primaryCode in( select parentcode 
							from productheirarchy p 
							inner join code c on c.code = p.primaryCode
							where c.category in  ('PCE', 'PCF', 'PCO', 'PCW') 
							and p.catalogType = '02')			
		
	
	Select distinct ph.primaryCode as Category,ph.primaryCode + ': ' + ph.CodeDescription as CategoryDescr,ParentCode 
	from ProductHeirarchy ph 
	where ph.CatalogType = '02'		-- category
		and CodeStatus='A'			-- Active
		and exists (select code from code where code.code = ph.primarycode and category in  ('PCE', 'PCF', 'PCO', 'PCW'))
	

	Select distinct ph.primaryCode as Class,ph.primaryCode + ': ' + ph.CodeDescription as ClassDescr,ParentCode  
	from StockInfo s INNER JOIN ProductHeirarchy ph on ISNULL(s.Class,'')=ph.primaryCode 
	where ph.CatalogType = '03'		-- class
		and CodeStatus='A'			-- Active


		
	
	Select distinct ph.primaryCode as SubClass,ph.primaryCode + ': ' + ph.CodeDescription as SubClassDescr,ParentCode 
	from StockInfo s INNER JOIN ProductHeirarchy ph on ISNULL(s.SubClass,'')=ph.primaryCode 
	where ph.CatalogType = '04'		-- subclass
		and CodeStatus='A'			-- Active
	
	
	-- Associated Items
	select ISNULL(pg.CodeDescription,'Any') as ProductGroupDescr,ISNULL(cat.CodeDescription,'Any') as CategoryDescr,
		ISNULL(cl.CodeDescription,'Any') as ClassDescr,ISNULL(sc.CodeDescription,'Any') as SubClassDescr,
		ProductGroup, a.Category, a.Class, a.SubClass,s.IUPC as AssociatedItem,s.itemdescr1 as 'Description1',s.itemdescr2 as 'Description2',
		AssocItemId as ItemId 
	from StockInfoAssociated a LEFT outer JOIN ProductHeirarchy pg on a.ProductGroup=pg.PrimaryCode and pg.CatalogType = '01'		-- Product Group
					LEFT outer JOIN  ProductHeirarchy cat on a.Category=cat.PrimaryCode and cat.CatalogType = '02'		-- category
					LEFT outer JOIN  ProductHeirarchy cl on a.Class=cl.PrimaryCode and cl.CatalogType = '03'			-- class
					LEFT outer JOIN  ProductHeirarchy sc on a.SubClass=sc.PrimaryCode and sc.CatalogType = '04'			-- subclass
					INNER JOIN Stockinfo s on a.AssocItemId=s.ID
	order by ProductGroup, a.Category, a.Class, a.SubClass,s.IUPC
	
	
	set @return=@@ERROR

go
	
-- End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  
