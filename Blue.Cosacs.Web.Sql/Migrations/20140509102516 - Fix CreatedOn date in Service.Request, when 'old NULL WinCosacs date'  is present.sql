DECLARE @MinPkDate DATETIME = (SELECT MIN(PK_Date) FROM [Time])
UPDATE Service.Request
SET CreatedOn = ISNULL(ResolutionDate, @MinPkDate)
WHERE CreatedOn = '19000101' -- the date '19000101' is used as a NULL date in WinCosacs
