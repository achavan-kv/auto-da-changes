DECLARE @strSQL NVARCHAR(512)

DECLARE constraints CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR
	SELECT 
		'ALTER TABLE Courtsperson DROP CONSTRAINT ' + c.name
	FROM 
		sys.default_constraints c 
		INNER JOIN sys.columns col
			ON c.parent_column_id = col.column_id
			AND col.object_id = c.parent_object_id
			AND c.parent_object_id = OBJECT_ID('dbo.courtsperson')
	WHERE	
		col.name IN ('dutyfree', 'loggedIn', 'loggedInAt', 'FactEmployeeNo', 'MachineLoggedOn', 'empeechange')
OPEN constraints

FETCH NEXT FROM constraints 
INTO @strSQL

WHILE @@FETCH_STATUS = 0
BEGIN

	EXEC SP_EXECUTESQL @strSQL

	FETCH NEXT FROM constraints 
    INTO @strSQL
END 

CLOSE constraints;
DEALLOCATE constraints;

IF EXISTS(SELECT 1 FROM sys.Columns c WHERE c.object_id = OBJECT_ID('dbo.courtsperson') AND c.name = 'dutyfree')
	ALTER TABLE dbo.courtsperson
		DROP COLUMN dutyfree, loggedIn, loggedInAt, FactEmployeeNo, MachineLoggedOn, empeechange
GO

