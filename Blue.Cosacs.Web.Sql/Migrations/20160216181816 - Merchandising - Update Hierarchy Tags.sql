-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Merchandising.HierarchyTag SET Name = 'MOTOR COMPREHENSIVE 1' WHERE Code = 'A187' 
UPDATE Merchandising.HierarchyTag SET Name = 'MOTOR COMPREHENSIVE 2' WHERE Code = 'A188' 
UPDATE Merchandising.HierarchyTag SET Name = 'MOTOR COMPREHENSIVE 3' WHERE Code = 'A189' 
UPDATE Merchandising.HierarchyTag SET Name = 'NON-STOCKS MISCELLANEOUS' WHERE Code = 'V4B' 
UPDATE Merchandising.HierarchyTag SET Name = 'NON-STOCKS OTHERS' WHERE Code = 'S2A'