-- transaction: true

IF OBJECT_ID('tempdb..#TmpRequestPartCostPrice') IS NOT NULL
BEGIN
    DROP TABLE #TmpRequestPartCostPrice    
END

IF EXISTS(SELECT * FROM syscolumns WHERE name='CostPrice' AND object_name(id)='RequestPart')
BEGIN

    SELECT Id, RequestId, CostPrice
    INTO #TmpRequestPartCostPrice
    FROM Service.RequestPart


    IF EXISTS(SELECT * FROM syscolumns WHERE name='CostPrice' AND object_name(id)='RequestPart')
    BEGIN
        ALTER TABLE Service.RequestPart
        DROP COLUMN CostPrice
    END


    IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='CostPrice' AND object_name(id)='RequestPart')
    BEGIN
        ALTER TABLE Service.RequestPart
        ADD CostPrice dbo.BlueAmount NULL
    END


    UPDATE RP
    SET CostPrice = tmp.CostPrice
    FROM Service.RequestPart RP
    JOIN #TmpRequestPartCostPrice tmp
        ON RP.id = tmp.id AND RP.RequestId = tmp.RequestId
    WHERE tmp.CostPrice IS NOT NULL

END
