IF EXISTS (SELECT * FROM syscolumns
           WHERE name = 'EmpeeType'
           AND OBJECT_NAME(id) = 'CMActionRights')

BEGIN

SELECT EmpeeNo ,
        Strategy ,
        Action ,
        MAX(Empeenochange) AS Empeenochange,
        CAST(MAX(CAST(CycleToNextFlag as INT)) AS BIT) AS CycleToNextFlag, 
        CAST(MAX(CAST(MinNotesLength as INT)) AS BIT) AS MinNotesLength 
INTO #temp
FROM dbo.CMActionRights
GROUP BY EmpeeNo, Strategy, Action

DELETE FROM dbo.CMActionRights

ALTER TABLE dbo.CMActionRights
	DROP CONSTRAINT PK_CMActionRights
END 
GO

IF EXISTS (SELECT * FROM syscolumns
           WHERE name = 'EmpeeType'
           AND OBJECT_NAME(id) = 'CMActionRights')

BEGIN
ALTER TABLE dbo.CMActionRights ADD CONSTRAINT
	PK_CMActionRights PRIMARY KEY CLUSTERED 
	(
	EmpeeNo,
	Strategy,
	Action
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO
IF EXISTS (SELECT * FROM syscolumns
           WHERE name = 'EmpeeType'
           AND OBJECT_NAME(id) = 'CMActionRights')

BEGIN
ALTER TABLE dbo.CMActionRights
	DROP COLUMN EmpeeType
END
GO

ALTER TABLE dbo.CMActionRights SET (LOCK_ESCALATION = TABLE)
GO

IF object_id('tempdb..#temp') IS NOT NULL
BEGIN
INSERT INTO dbo.CMActionRights
        ( EmpeeNo ,
          Strategy ,
          Action ,
          Empeenochange ,
          CycleToNextFlag ,
          MinNotesLength
        )
SELECT EmpeeNo ,
          Strategy ,
          Action ,
          Empeenochange ,
          CycleToNextFlag ,
          MinNotesLength FROM #temp
END
