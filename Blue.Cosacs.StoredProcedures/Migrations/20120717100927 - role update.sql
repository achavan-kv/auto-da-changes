UPDATE Admin.Role
SET Description = name 
WHERE Description = ''
GO

ALTER TABLE Admin.Role
ALTER COLUMN name VARCHAR(100)
GO


SELECT * 
INTO #temp
FROM admin.Role

UPDATE #temp
SET description = description + CONVERT(VARCHAR(100),(SELECT CASE WHEN 1=1 THEN (SELECT COUNT(*) FROM #temp a 
										    WHERE a.description = #temp.description 
										    AND a.ID < #temp.ID) END ))

UPDATE admin.ROLE
SET NAME = CASE WHEN SUBSTRING(#temp.DESCRIPTION,LEN(#temp.DESCRIPTION) , 1) != 0 
           THEN #temp.DESCRIPTION
           ELSE SUBSTRING(#temp.DESCRIPTION,0 , LEN(#temp.DESCRIPTION) ) END
FROM #temp
WHERE #temp.id = admin.ROLE.id



ALTER TABLE Admin.Role
DROP COLUMN [Description]
GO


