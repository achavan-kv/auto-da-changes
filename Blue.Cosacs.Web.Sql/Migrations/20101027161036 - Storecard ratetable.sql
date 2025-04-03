
 CREATE TABLE StoreCardRates
	(
        Id int identity(1,1),
		DateApplied DATETIME NOT NULL,
		RateName VARCHAR(20) NOT NULL,
		ScoreFrom INT NOT NULL,
		ScoreTo INT NOT NULL,
		Retail FLOAT NOT NULL,
		Cash FLOAT NOT NULL
	)
  GO
  

  
  CREATE TABLE StoreCardRatesAudit
  (
        Id int identity(1,1),
		DateInserted DATETIME NOT NULL,
		DateApplied DATETIME NOT NULL,
		RateName VARCHAR(20) NOT NULL,
		ScoreFrom INT NOT NULL,
		ScoreTo INT NOT NULL,
		Retail FLOAT NOT NULL,
		Cash FLOAT NOT NULL
  )
 GO
   
          
CREATE CLUSTERED INDEX idx_SC_Rates 
ON  StoreCardRates (DateApplied, RateName)
GO

CREATE CLUSTERED INDEX idx_SC_Rates_Audit 
ON  StoreCardRatesAudit (DateApplied, RateName)
GO


