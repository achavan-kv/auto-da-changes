-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE	Merchandising.HierarchyTag 
SET		Name = 'Repo Elec Goods'
WHERE	Code = '13X'

UPDATE	Merchandising.HierarchyTag 
SET		Name = 'Repo Furniture Goods'
WHERE	Code = '13Y'

UPDATE	Merchandising.HierarchyTag 
SET		Name = 'Repo PC''s Goods'
WHERE	Code = '13Z'

UPDATE	Merchandising.HierarchyTag
SET		Code = 133
WHERE	Code = 131 AND Name = 'REPO PCS'
