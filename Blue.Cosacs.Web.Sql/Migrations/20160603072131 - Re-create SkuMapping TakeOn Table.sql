-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT OBJECT_ID('Merchandising.SkuMapping'))
BEGIN 
    SELECT * INTO #Data FROM Merchandising.SkuMapping

    DROP TABLE Merchandising.SkuMapping
END
GO

CREATE TABLE Merchandising.SkuMapping
(
	NewSku VARCHAR(18) NOT NULL,
	OldSku VARCHAR(18) NOT NULL
)
GO

INSERT INTO Merchandising.SkuMapping
SELECT * FROM #Data

DROP TABLE #Data