IF OBJECT_ID('Payments.ActiveExchangeRates') IS NOT NULL
	DROP VIEW Payments.ActiveExchangeRates
GO

CREATE VIEW Payments.ActiveExchangeRates
AS

SELECT r.id, c.CurrencyCode, c.CurrencyName, r.Rate, r.DateFrom, r.CreatedOn, r.CreatedBy FROM (

    -- get currencies removing all past/future entries (get just the current rate entries)
    SELECT CurrencyCode, MAX(DateFrom) AS DateFrom
    FROM Payments.ExchangeRate
    WHERE DateFrom <= CAST(GETDATE() AS DATE)
    GROUP BY CurrencyCode

) currentRates
INNER JOIN Payments.ExchangeRate r
    ON r.CurrencyCode = currentRates.CurrencyCode
    AND r.DateFrom = currentRates.DateFrom
INNER JOIN Payments.CurrencyCodes c
    ON r.CurrencyCode = c.CurrencyCode
