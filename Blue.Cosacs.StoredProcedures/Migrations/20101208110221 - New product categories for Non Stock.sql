-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Create new CodeCat Categories for Discounts and Miscellaneous
if not exists(select * from codecat where category='PCDIS')
Begin
insert into codecat (origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint,
			 CodeHeaderText, DescriptionHeaderText, SortOrderHeaderText, ReferenceHeaderText,
			 AdditionalHeaderText, ToolTipText)
Values (0,'PCDIS','Discounts',2,'Y','N','Y',
		'Product Category',null,null,null,null,null)
		
End
		
--insert into codecat (origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint,
--			 CodeHeaderText, DescriptionHeaderText, SortOrderHeaderText, ReferenceHeaderText,
--			 AdditionalHeaderText, ToolTipText)
--Values (0,'PCM','Miscellaneous',2,'Y','N','Y',
--		'Product Category',null,null,null,null,null)
		
-- If stock category not in  - PCE,PCF,PCO,PCW and not a discount add to PCO
insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference,additional)
select distinct 0,'PCO',i.category,'Category'+CAST(i.category as CHAR(3)), 'L',0,0,null
from stockinfo i
where not exists(select code from code c where c.code=i.category and c.category in('pce','pcf','pcw','pco'))
and not exists(select reference from code c where c.reference=i.category and c.category in('DIS'))
and i.category is not null

-- Discounts - PCD
insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference,additional)
select distinct 0,'PCDIS',i.category,'DiscountCategory'+CAST(i.category as CHAR(3)), 'L',0,0,null
from stockinfo i
where not exists(select code from code c where c.code=i.category and c.category in('pce','pcf','pcw','pco'))
and exists(select reference from code c where c.reference=i.category and c.category in('DIS')			
		and not exists(select code from code c2 where c.reference=c2.code and c2.category in('PCDIS'))	)


