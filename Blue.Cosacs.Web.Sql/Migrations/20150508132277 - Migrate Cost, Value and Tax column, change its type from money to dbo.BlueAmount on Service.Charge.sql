-- transaction: true

IF OBJECT_ID('tempdb..#TmpChargesMoneyValues') IS NOT NULL
BEGIN
    DROP TABLE #TmpChargesMoneyValues    
END


SELECT Id, RequestId, [Cost], [Value], [Tax]
INTO #TmpChargesMoneyValues
FROM Service.Charge


-- Migrate column Cost
IF EXISTS(SELECT * FROM syscolumns WHERE name='Cost' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
BEGIN

    ALTER TABLE Service.Charge
    DROP COLUMN Cost

    IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='Cost' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
    BEGIN
        ALTER TABLE Service.Charge
        ADD Cost dbo.BlueAmount NULL
    END

    UPDATE C
    SET Cost = tmp.Cost
    FROM Service.Charge C
    JOIN #TmpChargesMoneyValues tmp
        ON C.id = tmp.id AND C.RequestId = tmp.RequestId
	WHERE tmp.Cost IS NOT NULL

END


-- Migrate column Value
IF EXISTS(SELECT * FROM syscolumns WHERE name='Value' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
BEGIN

    ALTER TABLE Service.Charge
    DROP COLUMN [Value]

    IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='Value' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
    BEGIN
        ALTER TABLE Service.Charge
        ADD [Value] dbo.BlueAmount NOT NULL CONSTRAINT DF_Service_Charge_Value DEFAULT 0
    END

    UPDATE C
    SET [Value] = tmp.[Value]
    FROM Service.Charge C
    JOIN #TmpChargesMoneyValues tmp
        ON C.id = tmp.id AND C.RequestId = tmp.RequestId

END


-- Migrate column Tax
IF EXISTS(SELECT * FROM syscolumns WHERE name='Tax' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
BEGIN

    ALTER TABLE Service.Charge
    DROP COLUMN Tax

    IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='Tax' AND object_schema_name(id)='Service' AND object_name(id)='Charge')
    BEGIN
        ALTER TABLE Service.Charge
        ADD Tax dbo.BlueAmount NULL
    END

    UPDATE C
    SET Tax = tmp.Tax
    FROM Service.Charge C
    JOIN #TmpChargesMoneyValues tmp
        ON C.id = tmp.id AND C.RequestId = tmp.RequestId
	WHERE tmp.Tax IS NOT NULL

END
