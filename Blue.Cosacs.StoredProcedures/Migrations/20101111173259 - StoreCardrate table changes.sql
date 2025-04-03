

ALTER TABLE StoreCardrate
DROP COLUMN scorefrom
GO

ALTER TABLE StoreCardrate
DROP COLUMN scoreto
GO

ALTER TABLE StoreCardrate
DROP COLUMN RetailRateFixed
GO

ALTER TABLE StoreCardrate
DROP COLUMN RetailRateVariable
GO


CREATE TABLE StoreCardRateDetails
(
    Id INT PRIMARY KEY,
	ParentID INT,
	ScoreFrom INT,
	ScoreTo INT,
	RetailRateFixed interest_rate,
	RetailRateVariable interest_rate
	FOREIGN KEY (ParentID) REFERENCES StoreCardRate(Id)
)
GO
