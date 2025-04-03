-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO Merchandising.HierarchyTag (LevelId, Name, FirstYearWarrantyProvision, Code, AgedAfter)
VALUES 
	(2, 'LUGGAGE', NULL, '625', NULL), 
	(3, 'LUGGAGE CLASS', NULL, '62A', NULL)

INSERT INTO Merchandising.ClassMapping (ClassCode, LegacyCode)
VALUES ('62A', '517')