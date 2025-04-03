IF NOT EXISTS (SELECT * FROM sysobjects
			   WHERE name = 'StoreCardActivation'
			   AND xtype = 'U')
BEGIN
	CREATE TABLE StoreCardActivation
	(
		 CardNo BIGINT NOT NULL,
		 ActivationDate DATETIME NOT NULL,
		 ProofAddress VARCHAR(200) NOT NULL,
         ProofAddNotes VARCHAR(MAX) NULL,
         ProofID VARCHAR(200) NOT NULL,
         ProofIDNotes VARCHAR(MAX) NULL,
         SecurityQ VARCHAR(200) NOT NULL,
         SecurityA VARCHAR(MAX) NOT NULL
			CONSTRAINT [pk_StoreCardActivation] PRIMARY KEY NONCLUSTERED 
			(
				CardNo ASC
			)
	) 
END
GO

CREATE UNIQUE CLUSTERED INDEX idx_acctno ON storecardactivation (CardNo)
GO

ALTER TABLE StoreCardActivation 
ADD CONSTRAINT fk_CardNo FOREIGN KEY (CardNo) REFERENCES storecard(CardNumber);
GO

