
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='HierarchyVw')
	DROP VIEW HierarchyVw 
GO 
  
CREATE VIEW HierarchyVw 
-- **********************************************************************
-- Title: HierarchyVw.sql
-- Developer: John Croft
-- Date: 9th September 2011
-- Purpose: Hierarchy view

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- **********************************************************************

AS

select c.category as ProductGroup,case
			when c.category='PCE' then 'Electrical'
			when c.category='PCF' then 'Furniture'
			when c.category='PCW' then 'Workstation'
			when c.category='PCDIS' then 'Discounts'			 
			when c.category='PCO' then 'Other' end as ProductGroupDescr,c.Code as Department,
			c.codedescript as DeptDescr,h2.PrimaryCode as Class,h2.Codedescription as ClassDescr,
										ISNULL(h3.PrimaryCode,'') as SubClass,ISNULL(h3.Codedescription,'') as SubClassDescr
			
			 from code c INNER JOIN  productheirarchy h on c.code=h.primarycode and h.catalogtype='02'		-- Department
						and CAST(c.code as INT)>100	and c.category in('pce','pcf','pcw','pco') --h.parentcode in('6','7','8','R')
				INNER JOIN  productheirarchy h2 on h2.parentcode=h.primarycode and h2.catalogtype='03'		-- Class
				LEFT outer JOIN  productheirarchy h3 on h3.parentcode=h2.primarycode and h3.catalogtype='04'	-- SubClass

--where exists(select * from stockinfo s where s.category =c.code)		-- Department/Category exists

Go
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 