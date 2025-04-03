-- transaction: true

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock'
           AND Column_Name = 'ShortDescription'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    UPDATE NonStocks.NonStock
    SET ShortDescription = LEFT(LTRIM(RTRIM(ShortDescription)), 25)
    WHERE LEN(LTRIM(RTRIM(ShortDescription))) > 25

    -- Reduce column from varchar(32) to varchar(25)
    ALTER TABLE NonStocks.NonStock ALTER COLUMN ShortDescription VARCHAR(25) NOT NULL
END
