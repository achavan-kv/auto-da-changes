-- Script for Address Standardization CR2019 - 025

IF NOT EXISTS (
			SELECT 1
			FROM sys.Tables WITH (NOLOCK)
			WHERE NAME = 'AddressMaster'
				AND type = 'U'
			)
		
BEGIN
CREATE TABLE dbo.AddressMaster(
		Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		Region VARCHAR(50),
		Village VARCHAR(50),
		ZipCode VARCHAR(10)
	)
END
GO