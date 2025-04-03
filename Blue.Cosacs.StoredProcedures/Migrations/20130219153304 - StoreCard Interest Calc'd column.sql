ALTER TABLE dbo.StoreCardStatement
ADD InterestCalculatedDate DATETIME NULL
GO

DECLARE @intfree INT

SELECT @intfree = value FROM dbo.CountryMaintenance
WHERE CodeName ='SCardInterestFreeDays'

UPDATE dbo.StoreCardStatement
SET InterestCalculatedDate = DATEADD(day,@intfree,StoreCardStatement.DateTo)
FROM dbo.StoreCardPaymentDetails pd
WHERE pd.acctno = dbo.StoreCardStatement.Acctno
AND pd.DatePaymentDue > dbo.StoreCardStatement.DateTo

