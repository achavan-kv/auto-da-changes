-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT (
        EXISTS (SELECT 'a' FROM dbversion WHERE [version] = '10.0.1F') 
            OR 
        EXISTS (SELECT 'a' FROM Merchandising.HierarchyTag 
                WHERE Name = 'DISCOUNTS' and LevelId = 1 and Code = '14')
       )
BEGIN

    UPDATE Merchandising.HierarchyTag
    SET Code = '14'
    WHERE Code = 'R'
        AND LevelId = 1

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
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R79', 'TELEPHONES ACCESSORIES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R78', 'WIRE AND CABLE', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R77', 'PROJECTORS AND BOARDS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R76', 'TRANSISTORS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R75', 'SWITCHES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R74', 'CONECTORS AND PLUGS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R73', 'TRANSFORMADORES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R72', 'CAPACITORS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R71', 'RESISTANCE', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R70', 'ELECTRONIC PARTS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R68', 'PREMIUMS AND GIVE AWAY', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R65', 'RS CALCULATORS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R64', 'TOOLS AND HARDWARE', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R63', 'TIMERS CLOCKS WATCHES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R61', 'HOME ELECTRONICS AND DIMMER', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R60', 'TOYS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R49', 'SECURITY', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R44', 'TAPE AND ACCESORIES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R43', 'TELEPHONES AND INTERCOMS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R42', 'CHANGERS AND RECORDS ACC', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R40', 'RS SPEAKERS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R33', 'MIKES AND HEADPHONES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R32', 'PA AMPLIFIERS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R31', 'RECEIVERS AMPS TUNERS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R28', 'KITS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R26', 'PRINTERS AND COMPUTER ACC', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R25', 'COMPUTERS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R23', 'RS BATTERIES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R22', 'TEST EQUIPMENT', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R21', 'COMM CBS WALKIE TALKIE', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R20', 'SCANNERS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R19', 'COMMUNICATION UHF VHF', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R17', 'CELLULAR', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R16', 'TELEVISIONS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R15', 'TV ANTENNAS AND  ACCESORIES', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R14', 'TAPE RECORDERS AND DECKS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R13', 'SHELF SYSTEMS', '10', 365 UNION ALL
    SELECT 'R', 'RADIO SHACK', '880', 'RADIOSHACK', 'R12', 'RS RADIOS', '10', 365 UNION ALL
    SELECT '5', 'COMPUTER AND OFFICE', '510', 'OFFICE SYSTEM', '51H', 'OFFICE SYSTEMS PBX', '411', 365 UNION ALL
    SELECT '3', 'MAJOR WHITES', '320', 'VENTILATION AND HEATING', '32N', 'COMMERCIAL AIR CONDITIONING', '510', 365


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
END