SELECT DISTINCT	WorkList ,
        Description
        INTO #temp
        FROM dbo.CMWorkList

DELETE FROM dbo.CMWorkList

IF EXISTS (SELECT * FROM sysobjects
			WHERE xtype = 'pk'
			AND name = 'pk_WorkList' )
BEGIN
    ALTER TABLE dbo.CMWorkList
    DROP CONSTRAINT pk_WorkList
END 
GO

IF EXISTS (SELECT * FROM sysobjects
			WHERE xtype = 'pk'
			AND name = 'pk_CMWorkList' )
BEGIN
    ALTER TABLE dbo.CMWorkList
    DROP CONSTRAINT pk_CMWorkList
END 
GO

ALTER TABLE dbo.CMWorkList ADD CONSTRAINT
	PK_CMWorkList PRIMARY KEY CLUSTERED 
	(
	WorkList
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CMWorkList SET (LOCK_ESCALATION = TABLE)
GO

INSERT INTO dbo.CMWorkList
        ( WorkList, Description, EmpeeType )
SELECT WorkList ,
        Description,'' FROM #temp



DROP TABLE #temp

