-- transaction: true

IF EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID(N'NonStocks.NonStock')
           AND name = N'UQ_NonStocks_NonStock_SKU')
BEGIN
    -- JUST DROP UNIQUE CONSTRAINT: UQ_NonStocks_NonStock_SKU
    ALTER TABLE NonStocks.NonStock DROP UQ_NonStocks_NonStock_SKU
END


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock' AND Column_Name = 'SKU'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    -- If this migration fails it's probably because repeated SKU's after the trimming
    -- This trimming is done so the next alter table statment, to reduce the SKU column size, doesn't fail
    UPDATE NonStocks.NonStock
    SET SKU = LEFT(LTRIM(RTRIM(SKU)),8)
    WHERE LEN(LTRIM(RTRIM(SKU))) > 8

    -- Reduce column from varchar(18) to varchar(8)
    ALTER TABLE NonStocks.NonStock ALTER COLUMN SKU VARCHAR(8) NOT NULL
END


IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID(N'NonStocks.NonStock')
               AND name = N'UQ_NonStocks_NonStock_SKU')
BEGIN
    -- JUST CREATE again the UNIQUE CONSTRAINT: UQ_NonStocks_NonStock_SKU
    ALTER TABLE NonStocks.NonStock ADD CONSTRAINT UQ_NonStocks_NonStock_SKU UNIQUE (SKU)
END
