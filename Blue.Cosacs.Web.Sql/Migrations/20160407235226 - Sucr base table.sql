CREATE TABLE Merchandising.SucrBase
(
	Id INT IDENTITY(1,1) NOT NULL,
	LocationId INT NOT NULL,
	SummaryType VARCHAR(20) NOT NULL,
	Units INT NOT NULL, 
	Value DBO.BlueAmount NOT NULL
	 CONSTRAINT [PK_Merchandising_SucrBase] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
