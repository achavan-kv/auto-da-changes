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


    --Add new Radio Shack Hierarchy
    DECLARE @levelId int

    SELECT 	
	    CONVERT(VARCHAR(4), '') AS DivisionCode, 
        CONVERT(VARCHAR(100), '') AS DivisionName,
        CONVERT(VARCHAR(4), '') AS DepartmentCode,
        CONVERT(VARCHAR(100), '') AS DepartmentName,
        CONVERT(VARCHAR(4), '') AS ClassCode,
        CONVERT(VARCHAR(100), '') AS ClassName,
        CONVERT(VARCHAR(3), '') AS LegacyCode,
        CONVERT(INT, 0) AS AgedAfter
    INTO #Hierarchy
    DELETE FROM #Hierarchy

    INSERT INTO #Hierarchy (DivisionCode, DivisionName, DepartmentCode, DepartmentName, ClassCode, ClassName, LegacyCode, AgedAfter)
    select	'E', 'ASHLEY', '780', 'ASHLEY ACCESSORY', 'E80', 'ASH ACCESSORY', '80', 365	UNION ALL
    select	'E', 'ASHLEY', '781', 'ASHLEY BEDROOM', 'E81', 'ASH BEDROOM', '70', 365	UNION ALL
    select	'E', 'ASHLEY', '782', 'ASHLEY CURIOS', 'E82', 'ASH CURIOS', '60', 365 UNION ALL
    select	'E', 'ASHLEY', '783', 'ASHLEY DINING', 'E83', 'ASH DINING', '60', 365 UNION ALL
    select	'E', 'ASHLEY', '784', 'ASHLEY ENTERTAINMENT AND WALL UNITS', 'E84', 'ASH ENTERTAINMENT AND WALL UNITS', '80', 365 UNION ALL
    select	'E', 'ASHLEY', '785', 'ASHLEY HOME OFFICE', 'E85', 'ASH HOME OFFICE', '80', 365	UNION ALL
    select	'E', 'ASHLEY', '786', 'ASHLEY LAMPS', 'E86', 'ASH LAMPS', '80', 365	UNION ALL
    select	'E', 'ASHLEY', '787', 'ASHLEY MATTRESS', 'E87', 'ASH MATTRESS', '75', 365 UNION ALL
    select	'E', 'ASHLEY', '788', 'ASHLEY OCCASIONAL TABLES', 'E88', 'ASH OCCASIONAL TABLES', '80', 365	UNION ALL
    select	'E', 'ASHLEY', '789', 'ASHLEY RUGS', 'E89', 'ASH RUGS', '81', 365 UNION ALL
    select	'E', 'ASHLEY', '790', 'ASHLEY UPHOLSTERY', 'E90', 'ASH UPHOLSTERY', '50', 365	


    -- Division
    SELECT @levelId = id FROM Merchandising.HierarchyLevel WHERE name = 'Division'
 
    INSERT INTO Merchandising.HierarchyTag (LevelId, Name, Code, AgedAfter)
    SELECT DISTINCT @levelId, h.DivisionName, h.DivisionCode, AgedAfter
    FROM #Hierarchy h
    WHERE NOT EXISTS (SELECT 'a' FROM Merchandising.HierarchyTag 
				      WHERE LevelId = @levelId 
					    AND Name = h.DivisionName 
					    AND Code = h.DivisionCode)
 
    -- Department
    SELECT @levelId = id FROM Merchandising.HierarchyLevel WHERE name = 'Department'
 
    INSERT INTO Merchandising.HierarchyTag (LevelId, Name, Code, AgedAfter)
    SELECT DISTINCT @levelId, h.DepartmentName, h.DepartmentCode, AgedAfter
    FROM #Hierarchy h
    WHERE NOT EXISTS (SELECT 'a' FROM Merchandising.HierarchyTag
				      WHERE LevelId = @levelId
					    AND Name = h.DepartmentName
					    AND Code = h.DepartmentCode)
 
    -- Class
    SELECT @levelId = id FROM Merchandising.HierarchyLevel WHERE name = 'Class'

    INSERT INTO Merchandising.HierarchyTag (LevelId, Name, Code, AgedAfter)
    SELECT DISTINCT @levelId, h.ClassName, h.ClassCode, AgedAfter
    FROM #Hierarchy h
    WHERE NOT EXISTS (SELECT 'a' FROM Merchandising.HierarchyTag
				      WHERE LevelId = @levelId
					    AND Name = h.ClassName
					    AND Code = h.ClassCode)
					
    -- ClassMapping

    INSERT INTO Merchandising.ClassMapping (ClassCode, LegacyCode)
    SELECT DISTINCT h.ClassCode, h.LegacyCode 
    FROM #Hierarchy h
    WHERE NOT EXISTS (SELECT 'a' FROM Merchandising.ClassMapping c
				      WHERE c.LegacyCode = h.LegacyCode
					    AND c.ClassCode = h.ClassCode)

    DROP TABLE #Hierarchy
