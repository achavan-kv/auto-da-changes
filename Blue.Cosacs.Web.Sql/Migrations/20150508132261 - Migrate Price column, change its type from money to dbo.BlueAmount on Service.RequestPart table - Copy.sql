-- transaction: true

IF OBJECT_ID('tempdb..#TmpRequestPartPrice') IS NOT NULL
BEGIN
    DROP TABLE #TmpRequestPartPrice    
END

IF EXISTS(SELECT * FROM syscolumns WHERE name='Price' AND object_name(id)='RequestPart')
BEGIN

    SELECT Id, RequestId, Price
    INTO #TmpRequestPartPrice
    FROM Service.RequestPart


    IF EXISTS(SELECT * FROM syscolumns WHERE name='Price' AND object_name(id)='RequestPart')
    BEGIN
        ALTER TABLE Service.RequestPart
        DROP COLUMN Price
    END


    IF NOT EXISTS(SELECT * FROM syscolumns WHERE name='Price' AND object_name(id)='RequestPart')
    BEGIN
        ALTER TABLE Service.RequestPart
        ADD Price dbo.BlueAmount NOT NULL CONSTRAINT DF_Service_RequestPart_Price DEFAULT 0
    END


    UPDATE RP
    SET Price = tmp.Price
    FROM Service.RequestPart RP
    JOIN #TmpRequestPartPrice tmp
        ON RP.id = tmp.id AND RP.RequestId = tmp.RequestId

END
